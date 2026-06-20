namespace SagradaFamilia.Application.Services;

using AutoMapper;
using BCrypt.Net;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Reporting.Excel.Documents;
using SagradaFamilia.Application.Settings;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicoService> _logger;

    public MedicoService(
        IMedicoRepository medicoRepository,
        IUsuarioRepository usuarioRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> appSettings,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MedicoService> logger)
    {
        _medicoRepository = medicoRepository;
        _usuarioRepository = usuarioRepository;
        _resetTokenRepository = resetTokenRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _appSettings = appSettings.Value;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicoDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando el listado general de médicos registrados.");

        var medicos = await _medicoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<MedicoDto.ListResponse>>(medicos);
    }

    public async Task<PagedResponse<MedicoDto.ListResponse>> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _medicoRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending);
        var data = _mapper.Map<IEnumerable<MedicoDto.ListResponse>>(items);
        return PagedResponse<MedicoDto.ListResponse>.Ok(data, total, page, pageSize);
    }

    public async Task<MedicoDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando detalle del médico con ID: {Id}", id);

        var medico = await _medicoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Médico", id);

        return _mapper.Map<MedicoDto.DetailResponse>(medico);
    }

    public async Task<MedicoDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("Recuperando perfil médico para el Usuario ID: {UsuarioId}", usuarioId);

        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Médico asociado al usuario", usuarioId);

        return _mapper.Map<MedicoDto.DetailResponse>(medico);
    }

    public async Task<MedicoDto.DetailResponse> ObtenerConPacientesAsync(int id)
    {
        _logger.LogInformation("Obteniendo médico ID: {Id} incluyendo su lista de pacientes asignados.", id);

        var medico = await _medicoRepository.ObtenerConPacientesAsync(id)
            ?? throw new NotFoundException("Médico", id);

        return _mapper.Map<MedicoDto.DetailResponse>(medico);
    }

    public async Task<MedicoDto.DetailResponse> CrearAsync(MedicoDto.Create request)
    {
        _logger.LogInformation("Iniciando registro de nuevo médico: {Nombre} {Apellido} con Email: {Email}",
            request.Nombre, request.Apellido, request.Email);

        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
        {
            _logger.LogWarning("Fallo en registro: El email {Email} ya se encuentra en uso.", request.Email);
            throw new BusinessException("Ya existe un médico registrado con ese correo electrónico.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var usuario = new Usuario
            {
                Email        = request.Email,
                PasswordHash = BCrypt.HashPassword(Guid.NewGuid().ToString()),
                RolId        = (int)RolEnum.Medico,
                Activo       = false
            };
            await _usuarioRepository.CrearAsync(usuario);
            _logger.LogDebug("Cuenta de usuario creada (inactiva) para {Email} con ID: {UId}", request.Email, usuario.Id);

            var medico = new Medico
            {
                UsuarioId    = usuario.Id,
                Nombre       = request.Nombre,
                Apellido     = request.Apellido,
                Especialidad = request.Especialidad,
                Telefono     = request.Telefono
            };
            var creado = await _medicoRepository.CrearAsync(medico);

            var token = new PasswordResetToken
            {
                UsuarioId       = usuario.Id,
                Token           = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                FechaExpiracion = DateTime.UtcNow.AddHours(48)
            };
            await _resetTokenRepository.CrearAsync(token);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Médico '{Nombre} {Apellido}' registrado (pendiente activación) con ID: {Id}",
                creado.Nombre, creado.Apellido, creado.Id);

            var link = $"{_appSettings.FrontendUrl}/activar-cuenta?token={token.Token}";
            var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync("CUENTA_MEDICO", new Dictionary<string, string>
            {
                ["NOMBRE"]   = request.Nombre,
                ["APELLIDO"] = request.Apellido,
                ["LINK"]     = link
            });
            await _emailService.EnviarAsync(request.Email, asunto, cuerpo);

            var medicoCompleto = await _medicoRepository.ObtenerPorIdAsync(creado.Id);
            return _mapper.Map<MedicoDto.DetailResponse>(medicoCompleto!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error crítico durante la creación del médico. Se revirtieron los cambios (Rollback).");
            throw new BusinessException("No se pudo completar el registro del médico.");
        }
    }

    public async Task<MedicoDto.DetailResponse> ActualizarAsync(int id, MedicoDto.Update request)
    {
        _logger.LogInformation("Actualizando información profesional del médico ID: {Id}", id);

        var medico = await _medicoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Médico", id);

        // Mapeo de actualización (Nombre, Apellido, Especialidad, Telefono)
        _mapper.Map(request, medico);

        var actualizado = await _medicoRepository.ActualizarAsync(medico);

        _logger.LogInformation("Médico ID: {Id} actualizado correctamente.", id);

        var medicoCompleto = await _medicoRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<MedicoDto.DetailResponse>(medicoCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Iniciando proceso de eliminación para el médico ID: {Id}", id);

        var medico = await _medicoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Médico", id);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _medicoRepository.EliminarAsync(medico.Id);
            await _usuarioRepository.EliminarAsync(medico.UsuarioId);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Médico ID: {Id} y su cuenta de usuario asociada han sido eliminados lógicamente.", id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar el médico ID: {Id}. Se realizó Rollback.", id);
            throw new BusinessException("No se pudo eliminar el registro del médico.");
        }
    }

    public async Task RestablecerPasswordAsync(int medicoId)
    {
        _logger.LogInformation("Solicitud de restablecimiento de contraseña para médico ID: {Id}", medicoId);

        var medico = await _medicoRepository.ObtenerPorIdAsync(medicoId)
            ?? throw new NotFoundException("Médico", medicoId);

        await _resetTokenRepository.InvalidarTokensAnterioresAsync(medico.UsuarioId);

        var token = new PasswordResetToken
        {
            UsuarioId = medico.UsuarioId,
            Token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            FechaExpiracion = DateTime.UtcNow.AddHours(24)
        };
        await _resetTokenRepository.CrearAsync(token);

        var link = $"{_appSettings.FrontendUrl}/nueva-clave?token={token.Token}";
        var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync("CAMBIO_CLAVE", new Dictionary<string, string>
        {
            ["NOMBRE"] = $"{medico.Nombre} {medico.Apellido}",
            ["LINK"]   = link
        });

        await _emailService.EnviarAsync(medico.Usuario.Email, asunto, cuerpo);
        _logger.LogInformation("Email de restablecimiento enviado a {Email} para médico ID: {Id}", medico.Usuario.Email, medicoId);
    }

    public async Task<byte[]> GenerarPlantillaAsync()
    {
        _logger.LogInformation("Generando plantilla Excel para importación masiva de médicos.");
        return new MedicosPlantillaDocument().GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelAsync()
    {
        _logger.LogInformation("Exportando listado de médicos a Excel.");
        var medicos = await _medicoRepository.ObtenerTodosAsync();
        var dtos = _mapper.Map<IEnumerable<MedicoDto.ListResponse>>(medicos);
        return new MedicosExportDocument(dtos).GenerarBytes();
    }

    public async Task<MedicoDto.ImportResultado> ImportarAsync(Stream archivoStream)
    {
        _logger.LogInformation("Iniciando importación masiva de médicos desde Excel.");
        var resultado = new MedicoDto.ImportResultado();

        XLWorkbook workbook;
        try { workbook = new XLWorkbook(archivoStream); }
        catch (Exception ex)
        {
            _logger.LogWarning("Archivo Excel inválido: {Error}", ex.Message);
            resultado.Errores.Add(new MedicoDto.ImportError { Fila = 0, Mensaje = "El archivo no es un Excel válido (.xlsx)." });
            return resultado;
        }

        using (workbook)
        {
            IXLWorksheet ws;
            try { ws = workbook.Worksheet("Medicos"); }
            catch
            {
                resultado.Errores.Add(new MedicoDto.ImportError { Fila = 0, Mensaje = "No se encontró la hoja 'Medicos'. Use la plantilla oficial." });
                return resultado;
            }

            const int DataStartRow = 7;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? DataStartRow - 1;

            for (int rowNum = DataStartRow; rowNum <= lastRow; rowNum++)
            {
                var row = ws.Row(rowNum);
                string nombre      = row.Cell(1).GetString().Trim();
                string apellido    = row.Cell(2).GetString().Trim();
                string email       = row.Cell(3).GetString().Trim().ToLowerInvariant();
                string telefono    = row.Cell(4).GetString().Trim();
                string especialidad = row.Cell(5).GetString().Trim();

                if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(apellido) && string.IsNullOrEmpty(email))
                    continue;

                resultado.TotalProcesadas++;
                var erroresFila = new List<string>();

                if (string.IsNullOrEmpty(nombre))           erroresFila.Add("Nombre es obligatorio");
                else if (nombre.Length > 100)               erroresFila.Add("Nombre excede 100 caracteres");

                if (string.IsNullOrEmpty(apellido))         erroresFila.Add("Apellido es obligatorio");
                else if (apellido.Length > 100)             erroresFila.Add("Apellido excede 100 caracteres");

                if (string.IsNullOrEmpty(email))
                    erroresFila.Add("Email es obligatorio");
                else if (!EsEmailValido(email))
                    erroresFila.Add("Email no tiene formato válido");
                else if (email.Length > 200)
                    erroresFila.Add("Email excede 200 caracteres");

                if (telefono.Length > 20) erroresFila.Add("Teléfono excede 20 caracteres");
                if (especialidad.Length > 200) erroresFila.Add("Especialidad excede 200 caracteres");

                if (erroresFila.Count > 0)
                {
                    resultado.Errores.Add(new MedicoDto.ImportError { Fila = rowNum, Mensaje = string.Join("; ", erroresFila) });
                    continue;
                }

                var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(email);

                if (usuarioExistente != null)
                {
                    if (usuarioExistente.RolId != (int)RolEnum.Medico)
                    {
                        resultado.Errores.Add(new MedicoDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' pertenece a un usuario con otro rol." });
                        continue;
                    }

                    var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioExistente.Id);
                    if (medico == null)
                    {
                        resultado.Errores.Add(new MedicoDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' existe pero no tiene perfil de médico." });
                        continue;
                    }

                    medico.Nombre      = nombre;
                    medico.Apellido    = apellido;
                    medico.Telefono    = string.IsNullOrEmpty(telefono)     ? null : telefono;
                    medico.Especialidad = string.IsNullOrEmpty(especialidad) ? null : especialidad;
                    await _medicoRepository.ActualizarAsync(medico);
                    resultado.Actualizados++;
                }
                else
                {
                    await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var usuario = new Usuario
                        {
                            Email        = email,
                            PasswordHash = BCrypt.HashPassword(Guid.NewGuid().ToString("N")[..8] + "Aa1!"),
                            RolId        = (int)RolEnum.Medico,
                            Activo       = true
                        };
                        await _usuarioRepository.CrearAsync(usuario);

                        var medico = new Medico
                        {
                            UsuarioId   = usuario.Id,
                            Nombre      = nombre,
                            Apellido    = apellido,
                            Telefono    = string.IsNullOrEmpty(telefono)     ? null : telefono,
                            Especialidad = string.IsNullOrEmpty(especialidad) ? null : especialidad
                        };
                        await _medicoRepository.CrearAsync(medico);
                        await _unitOfWork.CommitAsync();
                        resultado.Importados++;
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        _logger.LogError(ex, "Error al crear médico con email {Email} en fila {Fila}", email, rowNum);
                        resultado.Errores.Add(new MedicoDto.ImportError { Fila = rowNum, Mensaje = "Error interno al crear el médico." });
                    }
                }
            }
        }

        _logger.LogInformation(
            "Importación de médicos finalizada: {I} creados, {A} actualizados, {E} errores de {T} filas.",
            resultado.Importados, resultado.Actualizados, resultado.Errores.Count, resultado.TotalProcesadas);

        return resultado;
    }

    private static bool EsEmailValido(string email)
    {
        try { var a = new System.Net.Mail.MailAddress(email); return a.Address == email; }
        catch { return false; }
    }
}
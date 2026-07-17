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

public class PadreService : IPadreService
{
    private readonly IPadreRepository _padreRepository;
    private readonly INinoRepository _ninoRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PadreService> _logger;

    public PadreService(
        IPadreRepository padreRepository,
        INinoRepository ninoRepository,
        IMedicoRepository medicoRepository,
        IUsuarioRepository usuarioRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> appSettings,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PadreService> logger)
    {
        _padreRepository = padreRepository;
        _ninoRepository = ninoRepository;
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

    public async Task<IEnumerable<PadreDto.ListResponse>> ObtenerTodosAsync(int? medicoId = null)
    {
        _logger.LogInformation("Consultando el listado de representantes (Padres). MedicoId filtro: {MedicoId}", medicoId);

        var padres = await _padreRepository.ObtenerTodosAsync(medicoId);
        return _mapper.Map<IEnumerable<PadreDto.ListResponse>>(padres);
    }

    public async Task<PagedResponse<PadreDto.ListResponse>> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending, int? medicoId = null)
    {
        var (items, total) = await _padreRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending, medicoId);
        var data = _mapper.Map<IEnumerable<PadreDto.ListResponse>>(items);
        return PagedResponse<PadreDto.ListResponse>.Ok(data, total, page, pageSize);
    }

    public async Task<PadreDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando detalle del padre con ID: {Id}", id);

        var padre = await _padreRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Padre", id);

        return _mapper.Map<PadreDto.DetailResponse>(padre);
    }

    public async Task<PadreDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("Buscando perfil de padre para el Usuario ID: {UsuarioId}", usuarioId);

        var padre = await _padreRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Padre asociado al usuario", usuarioId);

        return _mapper.Map<PadreDto.DetailResponse>(padre);
    }

    public async Task<bool> PerteneceAMedicoAsync(int padreId, int medicoId)
    {
        _logger.LogDebug("Verificando si el padre {PadreId} pertenece al médico {MedicoId}", padreId, medicoId);
        return await _padreRepository.PerteneceAMedicoAsync(padreId, medicoId);
    }

    public async Task<PadreDto.DetailResponse> CrearAsync(PadreDto.Create request)
    {
        _logger.LogInformation("Iniciando proceso de registro para el padre: {Nombre} {Apellido}", request.Nombre, request.Apellido);

        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
        {
            _logger.LogWarning("Intento de registro fallido: El email {Email} ya está en uso.", request.Email);
            throw new BusinessException("El correo electrónico ya se encuentra registrado.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var usuario = new Usuario
            {
                Email    = request.Email,
                PasswordHash = BCrypt.HashPassword(Guid.NewGuid().ToString()),
                RolId    = (int)RolEnum.Padre,
                Activo   = false
            };
            await _usuarioRepository.CrearAsync(usuario);
            _logger.LogDebug("Cuenta de usuario creada (inactiva) para {Email} con ID: {UId}", request.Email, usuario.Id);

            var padre = new Padre
            {
                UsuarioId = usuario.Id,
                MedicoId  = request.MedicoId,
                Nombre    = request.Nombre,
                Apellido  = request.Apellido,
                Telefono  = request.Telefono
            };
            var creado = await _padreRepository.CrearAsync(padre);

            var token = new PasswordResetToken
            {
                UsuarioId       = usuario.Id,
                Token           = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                FechaExpiracion = DateTime.UtcNow.AddHours(48)
            };
            await _resetTokenRepository.CrearAsync(token);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Padre '{Nombre}' registrado (pendiente activación) con ID: {Id}", creado.Nombre, creado.Id);

            var link = $"{_appSettings.FrontendUrl}/activar-cuenta?token={token.Token}";
            var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync("CUENTA_PADRE", new Dictionary<string, string>
            {
                ["NOMBRE"]   = request.Nombre,
                ["APELLIDO"] = request.Apellido,
                ["LINK"]     = link
            });
            await _emailService.EnviarAsync(request.Email, asunto, cuerpo);

            var padreCompleto = await _padreRepository.ObtenerPorIdAsync(creado.Id);
            return _mapper.Map<PadreDto.DetailResponse>(padreCompleto!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error crítico durante la creación del padre. Se realizó Rollback.");
            throw new BusinessException("No se pudo completar el registro del representante.");
        }
    }

    public async Task<PadreDto.DetailResponse> ActualizarAsync(int id, PadreDto.Update request)
    {
        _logger.LogInformation("Actualizando datos del padre ID: {Id}", id);

        var padre = await _padreRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Padre", id);

        _mapper.Map(request, padre);

        var actualizado = await _padreRepository.ActualizarAsync(padre);

        _logger.LogInformation("Padre ID: {Id} actualizado correctamente.", id);

        var padreCompleto = await _padreRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<PadreDto.DetailResponse>(padreCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Iniciando eliminación del padre ID: {Id}", id);

        var padre = await _padreRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Padre", id);

        if (padre.Ninos.Count > 0)
        {
            var nombres = string.Join(", ", padre.Ninos.Select(n => $"{n.Nombre} {n.Apellido}"));
            throw new BusinessException(
                $"No se puede eliminar: tiene {padre.Ninos.Count} niño(s) activo(s) asignado(s) ({nombres}). Reasigna o elimina primero a los niños.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _padreRepository.EliminarAsync(padre.Id);
            await _usuarioRepository.EliminarAsync(padre.UsuarioId);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Padre ID: {Id} y su cuenta de usuario asociada han sido eliminados.", id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar el padre ID: {Id}. Se realizó Rollback.", id);
            throw new BusinessException("No se pudo eliminar el registro del representante.");
        }
    }

    public async Task CambiarEmailAsync(int padreId, string nuevoEmail)
    {
        _logger.LogInformation("Solicitud de cambio de email para padre ID: {Id}", padreId);

        var padre = await _padreRepository.ObtenerPorIdAsync(padreId)
            ?? throw new NotFoundException("Padre", padreId);

        var emailNormalizado = nuevoEmail.Trim().ToLowerInvariant();

        if (await _usuarioRepository.ExisteEmailAsync(emailNormalizado))
            throw new BusinessException("El correo electrónico ya está en uso por otra cuenta.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(padre.UsuarioId)
            ?? throw new NotFoundException("Usuario asociado al padre", padre.UsuarioId);

        usuario.Email = emailNormalizado;
        await _usuarioRepository.ActualizarAsync(usuario);

        _logger.LogInformation("Email del padre ID: {Id} actualizado a {Email}.", padreId, emailNormalizado);
    }

    public async Task CambiarMedicoAsync(int padreId, int nuevoMedicoId)
    {
        _logger.LogInformation("Solicitud de reasignación de médico para padre ID: {Id} -> Médico ID: {MedicoId}", padreId, nuevoMedicoId);

        var padre = await _padreRepository.ObtenerPorIdAsync(padreId)
            ?? throw new NotFoundException("Padre", padreId);

        var medico = await _medicoRepository.ObtenerPorIdAsync(nuevoMedicoId)
            ?? throw new NotFoundException("Médico", nuevoMedicoId);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            padre.MedicoId = medico.Id;
            await _padreRepository.ActualizarAsync(padre);

            foreach (var nino in padre.Ninos)
            {
                nino.MedicoId = medico.Id;
                await _ninoRepository.ActualizarAsync(nino);
            }

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al reasignar médico para padre ID: {Id}. Se realizó Rollback.", padreId);
            throw new BusinessException("No se pudo reasignar el médico del padre.");
        }

        _logger.LogInformation("Padre ID: {Id} y sus {Count} hijo(s) reasignados al médico ID: {MedicoId}.", padreId, padre.Ninos.Count, nuevoMedicoId);
    }

    public async Task RestablecerPasswordAsync(int padreId)
    {
        _logger.LogInformation("Solicitud de restablecimiento de contraseña para padre ID: {Id}", padreId);

        var padre = await _padreRepository.ObtenerPorIdAsync(padreId)
            ?? throw new NotFoundException("Padre", padreId);

        await _resetTokenRepository.InvalidarTokensAnterioresAsync(padre.UsuarioId);

        var token = new PasswordResetToken
        {
            UsuarioId = padre.UsuarioId,
            Token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            FechaExpiracion = DateTime.UtcNow.AddHours(24)
        };
        await _resetTokenRepository.CrearAsync(token);

        var link = $"{_appSettings.FrontendUrl}/nueva-clave?token={token.Token}";
        var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync("CAMBIO_CLAVE", new Dictionary<string, string>
        {
            ["NOMBRE"] = $"{padre.Nombre} {padre.Apellido}",
            ["LINK"]   = link
        });

        await _emailService.EnviarAsync(padre.Usuario.Email, asunto, cuerpo);
        _logger.LogInformation("Email de restablecimiento enviado a {Email} para padre ID: {Id}", padre.Usuario.Email, padreId);
    }

    public async Task<byte[]> GenerarPlantillaAsync()
    {
        _logger.LogInformation("Generando plantilla Excel para importación masiva de representantes.");
        return new PadresPlantillaDocument().GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelAsync(int? medicoId = null)
    {
        _logger.LogInformation("Exportando listado de representantes a Excel. MedicoId filtro: {MedicoId}", medicoId);
        var padres = await _padreRepository.ObtenerTodosAsync(medicoId);
        var dtos = _mapper.Map<IEnumerable<PadreDto.ListResponse>>(padres);
        return new PadresExportDocument(dtos).GenerarBytes();
    }

    public async Task<PadreDto.ImportResultado> ImportarAsync(Stream archivoStream, int medicoId)
    {
        _logger.LogInformation("Iniciando importación masiva de representantes desde Excel.");
        var resultado = new PadreDto.ImportResultado();

        XLWorkbook workbook;
        try { workbook = new XLWorkbook(archivoStream); }
        catch (Exception ex)
        {
            _logger.LogWarning("Archivo Excel inválido: {Error}", ex.Message);
            resultado.Errores.Add(new PadreDto.ImportError { Fila = 0, Mensaje = "El archivo no es un Excel válido (.xlsx)." });
            return resultado;
        }

        using (workbook)
        {
            IXLWorksheet ws;
            try { ws = workbook.Worksheet("Representantes"); }
            catch
            {
                resultado.Errores.Add(new PadreDto.ImportError { Fila = 0, Mensaje = "No se encontró la hoja 'Representantes'. Use la plantilla oficial." });
                return resultado;
            }

            const int DataStartRow = 7;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? DataStartRow - 1;

            for (int rowNum = DataStartRow; rowNum <= lastRow; rowNum++)
            {
                var row = ws.Row(rowNum);
                string nombre    = row.Cell(1).GetString().Trim();
                string apellido  = row.Cell(2).GetString().Trim();
                string email     = row.Cell(3).GetString().Trim().ToLowerInvariant();
                string telefono  = row.Cell(4).GetString().Trim();

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

                if (erroresFila.Count > 0)
                {
                    resultado.Errores.Add(new PadreDto.ImportError { Fila = rowNum, Mensaje = string.Join("; ", erroresFila) });
                    continue;
                }

                var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(email);

                if (usuarioExistente != null)
                {
                    if (usuarioExistente.RolId != (int)RolEnum.Padre)
                    {
                        resultado.Errores.Add(new PadreDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' pertenece a un usuario con otro rol." });
                        continue;
                    }

                    var padre = await _padreRepository.ObtenerPorUsuarioIdAsync(usuarioExistente.Id);
                    if (padre == null)
                    {
                        resultado.Errores.Add(new PadreDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' existe pero no tiene perfil de representante." });
                        continue;
                    }

                    padre.Nombre   = nombre;
                    padre.Apellido = apellido;
                    padre.Telefono = string.IsNullOrEmpty(telefono) ? null : telefono;
                    await _padreRepository.ActualizarAsync(padre);
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
                            PasswordHash = BCrypt.HashPassword(Guid.NewGuid().ToString()),
                            RolId        = (int)RolEnum.Padre,
                            Activo       = false
                        };
                        await _usuarioRepository.CrearAsync(usuario);

                        var padre = new Padre
                        {
                            UsuarioId = usuario.Id,
                            MedicoId  = medicoId,
                            Nombre    = nombre,
                            Apellido  = apellido,
                            Telefono  = string.IsNullOrEmpty(telefono) ? null : telefono
                        };
                        await _padreRepository.CrearAsync(padre);

                        var token = new PasswordResetToken
                        {
                            UsuarioId       = usuario.Id,
                            Token           = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                            FechaExpiracion = DateTime.UtcNow.AddHours(48)
                        };
                        await _resetTokenRepository.CrearAsync(token);

                        await _unitOfWork.CommitAsync();
                        resultado.Importados++;

                        await EnviarCorreoActivacionAsync("CUENTA_PADRE", email, nombre, apellido, token.Token, rowNum);
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        _logger.LogError(ex, "Error al crear representante con email {Email} en fila {Fila}", email, rowNum);
                        resultado.Errores.Add(new PadreDto.ImportError { Fila = rowNum, Mensaje = "Error interno al crear el representante." });
                    }
                }
            }
        }

        _logger.LogInformation(
            "Importación de representantes finalizada: {I} creados, {A} actualizados, {E} errores de {T} filas.",
            resultado.Importados, resultado.Actualizados, resultado.Errores.Count, resultado.TotalProcesadas);

        return resultado;
    }

    private async Task EnviarCorreoActivacionAsync(string codigoEvento, string email, string nombre, string apellido, string token, int fila)
    {
        try
        {
            var link = $"{_appSettings.FrontendUrl}/activar-cuenta?token={token}";
            var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync(codigoEvento, new Dictionary<string, string>
            {
                ["NOMBRE"]   = nombre,
                ["APELLIDO"] = apellido,
                ["LINK"]     = link
            });
            await _emailService.EnviarAsync(email, asunto, cuerpo);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo enviar el correo de activación a {Email} (fila {Fila} del import).", email, fila);
        }
    }

    private static bool EsEmailValido(string email)
    {
        try { var a = new System.Net.Mail.MailAddress(email); return a.Address == email; }
        catch { return false; }
    }
}
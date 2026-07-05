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

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IPadreRepository _padreRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IMedicoRepository medicoRepository,
        IPadreRepository padreRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> appSettings,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _medicoRepository = medicoRepository;
        _padreRepository = padreRepository;
        _resetTokenRepository = resetTokenRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _appSettings = appSettings.Value;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UsuarioDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo el listado global de usuarios del sistema.");

        var usuarios = await _usuarioRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<UsuarioDto.ListResponse>>(usuarios);
    }

    public async Task<PagedResponse<UsuarioDto.ListResponse>> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _usuarioRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending);
        var data = _mapper.Map<IEnumerable<UsuarioDto.ListResponse>>(items);
        return PagedResponse<UsuarioDto.ListResponse>.Ok(data, total, page, pageSize);
    }

    public async Task<UsuarioDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando detalle del usuario con ID: {Id}", id);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Usuario", id);

        return _mapper.Map<UsuarioDto.DetailResponse>(usuario);
    }

    public async Task<IEnumerable<UsuarioDto.ListResponse>> ObtenerPorRolIdAsync(int rolId)
    {
        _logger.LogInformation("Filtrando usuarios por Rol ID: {RolId}", rolId);

        var usuarios = await _usuarioRepository.ObtenerPorRolIdAsync(rolId);
        return _mapper.Map<IEnumerable<UsuarioDto.ListResponse>>(usuarios);
    }

    public async Task<UsuarioDto.DetailResponse> CrearAsync(UsuarioDto.Create request)
    {
        _logger.LogInformation("Iniciando creación de nuevo usuario con email: {Email}", request.Email);

        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
        {
            _logger.LogWarning("Fallo al crear usuario. El email {Email} ya existe.", request.Email);
            throw new BusinessException("El correo electrónico ya se encuentra registrado.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var usuario = new Usuario
            {
                Email        = request.Email,
                PasswordHash = BCrypt.HashPassword(Guid.NewGuid().ToString()),
                RolId        = request.RolId,
                Activo       = false
            };
            var creado = await _usuarioRepository.CrearAsync(usuario);

            var token = new PasswordResetToken
            {
                UsuarioId       = creado.Id,
                Token           = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                FechaExpiracion = DateTime.UtcNow.AddHours(48)
            };
            await _resetTokenRepository.CrearAsync(token);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Usuario {Email} creado (pendiente activación) con ID: {Id}", creado.Email, creado.Id);

            var link = $"{_appSettings.FrontendUrl}/activar-cuenta?token={token.Token}";
            var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync("CUENTA_ADMIN", new Dictionary<string, string>
            {
                ["EMAIL"] = request.Email,
                ["LINK"]  = link
            });
            await _emailService.EnviarAsync(request.Email, asunto, cuerpo);

            var usuarioCompleto = await _usuarioRepository.ObtenerPorIdAsync(creado.Id);
            return _mapper.Map<UsuarioDto.DetailResponse>(usuarioCompleto!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error crítico durante la creación del usuario. Se realizó Rollback.");
            throw new BusinessException("No se pudo completar el registro del usuario.");
        }
    }

    public async Task<UsuarioDto.DetailResponse> ActualizarAsync(int id, UsuarioDto.Update request)
    {
        _logger.LogInformation("Actualizando datos del usuario ID: {Id}", id);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Usuario", id);

        usuario.RolId = request.RolId;
        usuario.Activo = request.Activo;

        var actualizado = await _usuarioRepository.ActualizarAsync(usuario);

        _logger.LogInformation("Usuario ID: {Id} actualizado correctamente. Nuevo Rol ID: {RolId}, Activo: {Activo}",
            actualizado.Id, actualizado.RolId, actualizado.Activo);

        var usuarioCompleto = await _usuarioRepository.ObtenerPorIdAsync(actualizado.Id);

        return _mapper.Map<UsuarioDto.DetailResponse>(usuarioCompleto!);
    }

    public async Task ActualizarEstadoAsync(int id, bool activo, int currentUserId)
    {
        _logger.LogInformation("Intentando cambiar el estado del usuario ID: {Id} a Activo={Activo}", id, activo);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Usuario", id);

        if (!activo && id == currentUserId)
            throw new BusinessException("No puedes desactivar tu propia cuenta.");

        if (!activo && usuario.Rol.Nombre == "Administrador")
        {
            var adminsActivos = await _usuarioRepository.ContarAdministradoresActivosAsync();
            if (adminsActivos <= 1)
                throw new BusinessException("No puedes desactivar al único administrador activo del sistema.");
        }

        usuario.Activo = activo;
        await _usuarioRepository.ActualizarAsync(usuario);

        _logger.LogInformation("Estado del usuario {Email} cambiado con éxito a: {Estado}",
            usuario.Email, activo ? "Habilitado" : "Bloqueado");
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Iniciando proceso de eliminación para el usuario ID: {Id}", id);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Usuario", id);

        await _usuarioRepository.EliminarAsync(usuario.Id);

        _logger.LogInformation("Usuario ID: {Id} eliminado del sistema.", id);
    }

    public async Task<byte[]> GenerarPlantillaAsync()
    {
        _logger.LogInformation("Generando plantilla Excel para importación masiva de usuarios.");

        var medicos = await _medicoRepository.ObtenerTodosAsync();
        var opcionesMedicos = medicos
            .OrderBy(m => m.Apellido).ThenBy(m => m.Nombre)
            .Select(m => ($"{m.Nombre} {m.Apellido}", m.Usuario.Email))
            .ToList();

        return new UsuariosPlantillaDocument(opcionesMedicos).GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelAsync()
    {
        _logger.LogInformation("Exportando listado de usuarios a Excel.");
        var usuarios = await _usuarioRepository.ObtenerTodosAsync();
        var dtos = _mapper.Map<IEnumerable<UsuarioDto.ListResponse>>(usuarios);
        return new UsuariosExportDocument(dtos).GenerarBytes();
    }

    public async Task<UsuarioDto.ImportResultado> ImportarAsync(Stream archivoStream)
    {
        _logger.LogInformation("Iniciando importación masiva de usuarios desde Excel.");
        var resultado = new UsuarioDto.ImportResultado();

        XLWorkbook workbook;
        try { workbook = new XLWorkbook(archivoStream); }
        catch (Exception ex)
        {
            _logger.LogWarning("Archivo Excel inválido: {Error}", ex.Message);
            resultado.Errores.Add(new UsuarioDto.ImportError { Fila = 0, Mensaje = "El archivo no es un Excel válido (.xlsx)." });
            return resultado;
        }

        using (workbook)
        {
            IXLWorksheet ws;
            try { ws = workbook.Worksheet("Usuarios"); }
            catch
            {
                resultado.Errores.Add(new UsuarioDto.ImportError { Fila = 0, Mensaje = "No se encontró la hoja 'Usuarios'. Use la plantilla oficial." });
                return resultado;
            }

            const int DataStartRow = 7;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? DataStartRow - 1;

            for (int rowNum = DataStartRow; rowNum <= lastRow; rowNum++)
            {
                var row = ws.Row(rowNum);
                string rolStr       = row.Cell(1).GetString().Trim();
                string nombre       = row.Cell(2).GetString().Trim();
                string apellido     = row.Cell(3).GetString().Trim();
                string email        = row.Cell(4).GetString().Trim().ToLowerInvariant();
                string telefono     = row.Cell(5).GetString().Trim();
                string especialidad = row.Cell(6).GetString().Trim();
                string emailMedico  = ExtraerEmail(row.Cell(7).GetString());

                if (string.IsNullOrEmpty(rolStr) && string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(apellido) && string.IsNullOrEmpty(email))
                    continue;

                resultado.TotalProcesadas++;
                var erroresFila = new List<string>();

                RolEnum? rol = rolStr.Trim().ToLowerInvariant() switch
                {
                    "medico" => RolEnum.Medico,
                    "padre" => RolEnum.Padre,
                    _ => null
                };
                if (rol is null) erroresFila.Add("Rol debe ser 'Medico' o 'Padre'");

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

                Medico? medicoAsignado = null;
                if (rol == RolEnum.Padre)
                {
                    if (string.IsNullOrEmpty(emailMedico))
                    {
                        erroresFila.Add("Médico es obligatorio cuando el Rol es Padre");
                    }
                    else
                    {
                        var usuarioMedico = await _usuarioRepository.ObtenerPorEmailAsync(emailMedico);
                        medicoAsignado = usuarioMedico is not null
                            ? await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioMedico.Id)
                            : null;
                        if (medicoAsignado is null)
                            erroresFila.Add($"El médico '{emailMedico}' no existe");
                    }
                }

                if (erroresFila.Count > 0)
                {
                    resultado.Errores.Add(new UsuarioDto.ImportError { Fila = rowNum, Mensaje = string.Join("; ", erroresFila) });
                    continue;
                }

                var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(email);

                if (usuarioExistente != null)
                {
                    if (usuarioExistente.RolId != (int)rol!.Value)
                    {
                        resultado.Errores.Add(new UsuarioDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' pertenece a un usuario con otro rol." });
                        continue;
                    }

                    if (rol == RolEnum.Medico)
                    {
                        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioExistente.Id);
                        if (medico == null)
                        {
                            resultado.Errores.Add(new UsuarioDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' existe pero no tiene perfil de médico." });
                            continue;
                        }
                        medico.Nombre       = nombre;
                        medico.Apellido     = apellido;
                        medico.Telefono     = string.IsNullOrEmpty(telefono)     ? null : telefono;
                        medico.Especialidad = string.IsNullOrEmpty(especialidad) ? null : especialidad;
                        await _medicoRepository.ActualizarAsync(medico);
                    }
                    else
                    {
                        var padre = await _padreRepository.ObtenerPorUsuarioIdAsync(usuarioExistente.Id);
                        if (padre == null)
                        {
                            resultado.Errores.Add(new UsuarioDto.ImportError { Fila = rowNum, Mensaje = $"El email '{email}' existe pero no tiene perfil de representante." });
                            continue;
                        }
                        padre.Nombre   = nombre;
                        padre.Apellido = apellido;
                        padre.Telefono = string.IsNullOrEmpty(telefono) ? null : telefono;
                        padre.MedicoId = medicoAsignado!.Id;
                        await _padreRepository.ActualizarAsync(padre);
                    }
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
                            RolId        = (int)rol!.Value,
                            Activo       = false
                        };
                        await _usuarioRepository.CrearAsync(usuario);

                        if (rol == RolEnum.Medico)
                        {
                            var medico = new Medico
                            {
                                UsuarioId    = usuario.Id,
                                Nombre       = nombre,
                                Apellido     = apellido,
                                Telefono     = string.IsNullOrEmpty(telefono)     ? null : telefono,
                                Especialidad = string.IsNullOrEmpty(especialidad) ? null : especialidad
                            };
                            await _medicoRepository.CrearAsync(medico);
                        }
                        else
                        {
                            var padre = new Padre
                            {
                                UsuarioId = usuario.Id,
                                MedicoId  = medicoAsignado!.Id,
                                Nombre    = nombre,
                                Apellido  = apellido,
                                Telefono  = string.IsNullOrEmpty(telefono) ? null : telefono
                            };
                            await _padreRepository.CrearAsync(padre);
                        }

                        var token = new PasswordResetToken
                        {
                            UsuarioId       = usuario.Id,
                            Token           = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                            FechaExpiracion = DateTime.UtcNow.AddHours(48)
                        };
                        await _resetTokenRepository.CrearAsync(token);

                        await _unitOfWork.CommitAsync();
                        resultado.Importados++;

                        var codigoEvento = rol == RolEnum.Medico ? "CUENTA_MEDICO" : "CUENTA_PADRE";
                        await EnviarCorreoActivacionAsync(codigoEvento, email, nombre, apellido, token.Token, rowNum);
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        _logger.LogError(ex, "Error al crear usuario con email {Email} en fila {Fila}", email, rowNum);
                        resultado.Errores.Add(new UsuarioDto.ImportError { Fila = rowNum, Mensaje = "Error interno al crear el usuario." });
                    }
                }
            }
        }

        _logger.LogInformation(
            "Importación de usuarios finalizada: {I} creados, {A} actualizados, {E} errores de {T} filas.",
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

    // El combo de la plantilla escribe "Nombre Apellido - email"; se acepta también un email plano.
    private static string ExtraerEmail(string valorCelda)
    {
        var valor = valorCelda.Trim();
        int idx = valor.LastIndexOf(" - ", StringComparison.Ordinal);
        var email = idx >= 0 ? valor[(idx + 3)..] : valor;
        return email.Trim().ToLowerInvariant();
    }

    private static bool EsEmailValido(string email)
    {
        try { var a = new System.Net.Mail.MailAddress(email); return a.Address == email; }
        catch { return false; }
    }
}
namespace SagradaFamilia.Application.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> appSettings,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository;
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
}
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
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class PadreService : IPadreService
{
    private readonly IPadreRepository _padreRepository;
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
        _usuarioRepository = usuarioRepository;
        _resetTokenRepository = resetTokenRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _appSettings = appSettings.Value;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PadreDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando el listado general de representantes (Padres).");

        var padres = await _padreRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<PadreDto.ListResponse>>(padres);
    }

    public async Task<PagedResponse<PadreDto.ListResponse>> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _padreRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending);
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
                Email = request.Email,
                PasswordHash = BCrypt.HashPassword(request.Password),
                RolId = (int)RolEnum.Padre,
                Activo = true
            };
            await _usuarioRepository.CrearAsync(usuario);
            _logger.LogDebug("Cuenta de usuario creada para {Email} con ID: {UId}", request.Email, usuario.Id);

            var padre = new Padre
            {
                UsuarioId = usuario.Id,
                MedicoId = request.MedicoId,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Telefono = request.Telefono
            };
            var creado = await _padreRepository.CrearAsync(padre);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Padre '{Nombre}' registrado exitosamente con ID: {Id}", creado.Nombre, creado.Id);

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

        if (request.MedicoId.HasValue)
        {
            padre.MedicoId = request.MedicoId.Value;
        }

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
        var nombre = $"{padre.Nombre} {padre.Apellido}";
        var cuerpo = _emailTemplateService.GenerarResetPassword(nombre, link);

        await _emailService.EnviarAsync(padre.Usuario.Email, "Restablecimiento de contraseña", cuerpo);
        _logger.LogInformation("Email de restablecimiento enviado a {Email} para padre ID: {Id}", padre.Usuario.Email, padreId);
    }
}
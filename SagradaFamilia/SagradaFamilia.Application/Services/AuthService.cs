namespace SagradaFamilia.Application.Services;

using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogSistemaService _logSistema;
    private readonly ITokenService _tokenService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogSistemaService logSistema,
        ITokenService tokenService,
        IOptions<AppSettings> appSettings,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _resetTokenRepository = resetTokenRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logSistema = logSistema;
        _tokenService = tokenService;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<LoginDto.Response> LoginAsync(LoginDto.Request request)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email);

        if (usuario == null || !BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            _logger.LogWarning("Login fallido para el email: {Email}", request.Email);
            throw new UnauthorizedException("Credenciales incorrectas.");
        }

        if (!usuario.Activo)
        {
            _logger.LogWarning("Intento de login con cuenta inactiva: {Email}", request.Email);
            throw new UnauthorizedException("La cuenta está desactivada.");
        }

        var accessToken = _tokenService.GenerarAccessToken(usuario);
        var refreshToken = _tokenService.GenerarRefreshToken();

        await _refreshTokenRepository.CrearAsync(new RefreshToken
        {
            UsuarioId = usuario.Id,
            Token = refreshToken,
            FechaExpiracion = _tokenService.ObtenerFechaExpiracionRefreshToken()
        });

        if (usuario.Rol.Nombre is "Medico" or "Administrador")
        {
            await _logSistema.RegistrarEventoAsync(
                nivel: "Information",
                mensaje: $"Login exitoso — {usuario.Rol.Nombre}: {usuario.Email}",
                endpoint: "/api/auth/login",
                usuarioId: usuario.Id);
        }

        return new LoginDto.Response
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Id = usuario.Id,
            MedicoId = usuario.Medico?.Id,
            Nombre = usuario.Padre?.Nombre ?? usuario.Medico?.Nombre ?? "Administrador",
            Apellido = usuario.Padre?.Apellido ?? usuario.Medico?.Apellido ?? string.Empty,
            Rol = usuario.Rol.Nombre,
            Expiracion = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<LoginDto.Response> RefreshTokenAsync(RefreshTokenDto.Request request)
    {
        var token = await _refreshTokenRepository.ObtenerPorTokenAsync(request.RefreshToken)
            ?? throw new UnauthorizedException("Refresh token inválido.");

        if (token.Revocado || token.FechaExpiracion < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token inválido o expirado.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(token.UsuarioId)
            ?? throw new UnauthorizedException("Usuario no encontrado.");

        await _refreshTokenRepository.RevocarAsync(request.RefreshToken);

        var nuevoAccessToken = _tokenService.GenerarAccessToken(usuario);
        var nuevoRefreshToken = _tokenService.GenerarRefreshToken();

        await _refreshTokenRepository.CrearAsync(new RefreshToken
        {
            UsuarioId = usuario.Id,
            Token = nuevoRefreshToken,
            FechaExpiracion = _tokenService.ObtenerFechaExpiracionRefreshToken()
        });

        return new LoginDto.Response
        {
            AccessToken = nuevoAccessToken,
            RefreshToken = nuevoRefreshToken,
            Id = usuario.Id,
            MedicoId = usuario.Medico?.Id,
            Nombre = usuario.Padre?.Nombre ?? usuario.Medico?.Nombre ?? "Administrador",
            Apellido = usuario.Padre?.Apellido ?? usuario.Medico?.Apellido ?? string.Empty,
            Rol = usuario.Rol.Nombre,
            Expiracion = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task NuevaClaveAsync(NuevaClaveDto.Request request)
    {
        var resetToken = await _resetTokenRepository.ObtenerTokenActivoAsync(request.Token)
            ?? throw new BusinessException("El enlace de restablecimiento no es válido o ya expiró.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(resetToken.UsuarioId)
            ?? throw new NotFoundException("Usuario", resetToken.UsuarioId);

        usuario.PasswordHash = BCrypt.HashPassword(request.NuevaClave);
        await _usuarioRepository.ActualizarAsync(usuario);
        await _resetTokenRepository.MarcarUsadoAsync(resetToken);

        await _logSistema.RegistrarEventoAsync(
            nivel: "Information",
            mensaje: $"Contraseña restablecida exitosamente — Usuario: {usuario.Email}",
            endpoint: "/api/auth/nueva-clave",
            usuarioId: usuario.Id);
    }

    public async Task ActivarCuentaAsync(ActivarCuentaDto.Request request)
    {
        var resetToken = await _resetTokenRepository.ObtenerTokenActivoAsync(request.Token)
            ?? throw new BusinessException("El enlace de activación no es válido o ya expiró.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(resetToken.UsuarioId)
            ?? throw new NotFoundException("Usuario", resetToken.UsuarioId);

        usuario.PasswordHash = BCrypt.HashPassword(request.NuevaClave);
        usuario.Activo = true;
        await _usuarioRepository.ActualizarAsync(usuario);
        await _resetTokenRepository.MarcarUsadoAsync(resetToken);

        await _logSistema.RegistrarEventoAsync(
            nivel: "Information",
            mensaje: $"Cuenta activada exitosamente — Usuario: {usuario.Email}",
            endpoint: "/api/auth/activar-cuenta",
            usuarioId: usuario.Id);
    }

    public async Task SolicitarResetAsync(SolicitarResetDto.Request request)
    {
        // Siempre respondemos con éxito para no revelar si el email existe
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email);

        if (usuario is null || usuario.RolId != (int)RolEnum.Medico || !usuario.Activo)
        {
            _logger.LogWarning("Solicitud de reset ignorada para: {Email}", request.Email);
            return;
        }

        var medico = usuario.Medico
            ?? throw new NotFoundException("Perfil médico del usuario", usuario.Id);

        var token = new PasswordResetToken
        {
            UsuarioId       = usuario.Id,
            Token           = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            FechaExpiracion = DateTime.UtcNow.AddHours(24)
        };
        await _resetTokenRepository.CrearAsync(token);

        var link = $"{_appSettings.FrontendUrl}/nueva-clave?token={token.Token}";
        var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync("CAMBIO_CLAVE", new Dictionary<string, string>
        {
            ["NOMBRE"] = $"{medico.Nombre} {medico.Apellido}",
            ["LINK"]   = link
        });
        await _emailService.EnviarAsync(usuario.Email, asunto, cuerpo);

        await _logSistema.RegistrarEventoAsync(
            nivel: "Information",
            mensaje: $"Solicitud de restablecimiento de contraseña enviada a: {usuario.Email}",
            endpoint: "/api/auth/solicitar-reset",
            usuarioId: usuario.Id);
    }
}

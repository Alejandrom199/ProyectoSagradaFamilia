namespace SagradaFamilia.Application.Services;

using BCrypt.Net;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly ILogSistemaService _logSistema;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        ILogSistemaService logSistema,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _resetTokenRepository = resetTokenRepository;
        _logSistema = logSistema;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginDto.Response> LoginAsync(LoginDto.Request request)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email);

        if (usuario == null || !BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            // Login fallido → Warning, capturado automáticamente por DatabaseLogger
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
            FechaExpiracion = DateTime.UtcNow.AddDays(7)
        });

        // Registrar login exitoso de Médico y Administrador (con UsuarioId)
        // Los logins de Padre no se registran — no son relevantes para monitoreo
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
            Nombre = usuario.Padre != null ? $"{usuario.Padre.Nombre} {usuario.Padre.Apellido}" :
                     usuario.Medico != null ? $"{usuario.Medico.Nombre} {usuario.Medico.Apellido}" : "Administrador",
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
            FechaExpiracion = DateTime.UtcNow.AddDays(7)
        });

        return new LoginDto.Response
        {
            AccessToken = nuevoAccessToken,
            RefreshToken = nuevoRefreshToken,
            Nombre = usuario.Padre?.Nombre ?? usuario.Medico?.Nombre ?? "Admin",
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
}

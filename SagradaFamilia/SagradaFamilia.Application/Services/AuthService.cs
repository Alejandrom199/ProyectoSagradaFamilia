namespace SagradaFamilia.Application.Services;

using AutoMapper;
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
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginDto.Response> LoginAsync(LoginDto.Request request)
    {
        _logger.LogInformation("Intento de login: {Email}", request.Email);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Credenciales incorrectas.");

        if (!usuario.Activo) throw new UnauthorizedException("La cuenta está desactivada.");

        if (!BCrypt.Verify(request.Password, usuario.PasswordHash))
            throw new UnauthorizedException("Credenciales incorrectas.");

        var accessToken = _tokenService.GenerarAccessToken(usuario);
        var refreshToken = _tokenService.GenerarRefreshToken();

        await _refreshTokenRepository.CrearAsync(new RefreshToken
        {
            UsuarioId = usuario.Id,
            Token = refreshToken,
            FechaExpiracion = DateTime.UtcNow.AddDays(7)
        });

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
}
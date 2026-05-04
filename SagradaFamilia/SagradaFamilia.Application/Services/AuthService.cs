namespace SagradaFamilia.Application.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Intento de login: {Email}", request.Email);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Credenciales incorrectas.");

        if (!usuario.Activo)
            throw new UnauthorizedException("La cuenta está desactivada.");

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

        _logger.LogInformation("Login exitoso para usuario ID: {Id}", usuario.Id);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Nombre = $"{usuario.Nombre} {usuario.Apellido}",
            Rol = usuario.Rol.Nombre,
            Expiracion = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var token = await _refreshTokenRepository.ObtenerPorTokenAsync(request.RefreshToken)
            ?? throw new UnauthorizedException("Refresh token inválido.");

        if (token.Revocado)
            throw new UnauthorizedException("Refresh token revocado.");

        if (token.FechaExpiracion < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token expirado.");

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

        _logger.LogInformation("Refresh token renovado para usuario ID: {Id}", usuario.Id);

        return new LoginResponse
        {
            AccessToken = nuevoAccessToken,
            RefreshToken = nuevoRefreshToken,
            Nombre = $"{usuario.Nombre} {usuario.Apellido}",
            Rol = usuario.Rol.Nombre,
            Expiracion = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<UsuarioResponse> CrearPadreAsync(CrearPadreRequest request)
    {
        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
            throw new BusinessException("Ya existe un usuario con ese email.");

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Email = request.Email,
            PasswordHash = BCrypt.HashPassword(request.Password),
            RolId = (int)RolEnum.Padre,
            Telefono = request.Telefono,
            Activo = true
        };

        var creado = await _usuarioRepository.CrearAsync(usuario);
        _logger.LogInformation("Padre creado con ID: {Id}", creado.Id);

        return _mapper.Map<UsuarioResponse>(creado);
    }

    public async Task<IEnumerable<UsuarioResponse>> ObtenerPadresAsync()
    {
        var padres = await _usuarioRepository.ObtenerPorRolIdAsync((int)RolEnum.Padre);
        return _mapper.Map<IEnumerable<UsuarioResponse>>(padres);
    }

    public async Task EliminarPadreAsync(int id)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Usuario", id);

        if (usuario.RolId != (int)RolEnum.Padre)
            throw new BusinessException("Solo se pueden eliminar cuentas de padres.");

        await _usuarioRepository.EliminarAsync(id);
        _logger.LogInformation("Padre eliminado con ID: {Id}", id);
    }
}
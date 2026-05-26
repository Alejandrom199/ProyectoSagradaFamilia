namespace SagradaFamilia.Application.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository;
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

        var usuario = new Usuario
        {
            Email = request.Email,
            PasswordHash = BCrypt.HashPassword(request.Password),
            RolId = request.RolId,
            Activo = true 
        };

        var creado = await _usuarioRepository.CrearAsync(usuario);

        _logger.LogInformation("Usuario {Email} creado exitosamente con ID: {Id}", creado.Email, creado.Id);

        var usuarioCompleto = await _usuarioRepository.ObtenerPorIdAsync(creado.Id);

        return _mapper.Map<UsuarioDto.DetailResponse>(usuarioCompleto!);
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

    public async Task ActualizarEstadoAsync(int id, bool activo)
    {
        _logger.LogInformation("Intentando cambiar el estado del usuario ID: {Id} a Activo={Activo}", id, activo);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Usuario", id);

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
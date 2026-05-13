namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MenuService> _logger;

    public MenuService(
        IMenuRepository menuRepository,
        IMapper mapper,
        ILogger<MenuService> logger)
    {
        _menuRepository = menuRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MenuDto.MenuResponse>> ObtenerMenuPorUsuarioAsync(int usuarioId, int rolId)
    {
        _logger.LogInformation("Cargando menú dinámico para Usuario ID: {Id} con Rol ID: {Rol}", usuarioId, rolId);

        // Obtenemos los módulos y opciones permitidas desde el repositorio de dominio
        var modulos = await _menuRepository.ObtenerMenuPorUsuarioAsync(usuarioId, rolId);

        return _mapper.Map<IEnumerable<MenuDto.MenuResponse>>(modulos);
    }

    public async Task AsignarPermisoAsync(PermisoDto.AsignarRequest request)
    {
        var permiso = new UsuarioPermiso
        {
            UsuarioId = request.UsuarioId,
            OpcionAccionId = request.OpcionAccionId,
            Permitido = request.Permitido
        };

        await _menuRepository.CrearUsuarioPermisoAsync(permiso);

        _logger.LogInformation("Permiso asignado: Usuario {UId} -> Acción {AId} ({Status})",
            request.UsuarioId, request.OpcionAccionId, request.Permitido ? "Permitido" : "Denegado");
    }

    public async Task RevocarPermisoAsync(int usuarioId, int opcionAccionId)
    {
        await _menuRepository.EliminarUsuarioPermisoAsync(usuarioId, opcionAccionId);

        _logger.LogInformation("Permiso revocado para Usuario ID: {UId} en Acción ID: {AId}", usuarioId, opcionAccionId);
    }
}
namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
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

    public async Task<IEnumerable<PermisoDto.ModuloPermisoResponse>> ObtenerPermisosRolAsync(int rolId)
    {
        var modulos = await _menuRepository.ObtenerTodosLosModulosConOpcionesAsync();
        var permisos = await _menuRepository.ObtenerPermisosPorRolIdAsync(rolId);
        var permitidos = permisos.Where(p => p.Permitido).Select(p => p.OpcionAccionId).ToHashSet();

        return modulos.Select(m => new PermisoDto.ModuloPermisoResponse
        {
            ModuloId = m.Id,
            ModuloNombre = m.Nombre,
            Opciones = m.Opciones.Select(o => new PermisoDto.OpcionPermisoResponse
            {
                OpcionId = o.Id,
                OpcionNombre = o.Nombre,
                Acciones = o.OpcionAcciones.Select(oa => new PermisoDto.AccionPermisoResponse
                {
                    OpcionAccionId = oa.Id,
                    AccionNombre = oa.Accion.Nombre,
                    Permitido = permitidos.Contains(oa.Id)
                }).ToList()
            }).ToList()
        });
    }

    public async Task ActualizarPermisoRolAsync(int rolId, int opcionAccionId, bool permitido)
    {
        // Admin nunca se toca
        if (rolId == (int)RolEnum.Administrador)
            throw new InvalidOperationException("No se pueden modificar los permisos del Administrador.");

        await _menuRepository.ActualizarPermisosRolAsync(rolId, [opcionAccionId], permitido);
    }
}
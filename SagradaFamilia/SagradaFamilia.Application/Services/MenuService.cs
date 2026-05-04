namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
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

    public async Task<IEnumerable<MenuResponse>> ObtenerMenuPorUsuarioAsync(
        int usuarioId, int rolId)
    {
        _logger.LogInformation(
            "Obteniendo menú para usuario ID: {Id}, rol ID: {RolId}",
            usuarioId, rolId);

        var modulos = await _menuRepository.ObtenerMenuPorUsuarioAsync(usuarioId, rolId);
        return _mapper.Map<IEnumerable<MenuResponse>>(modulos);
    }

    public async Task AsignarPermisoAsync(AsignarPermisoRequest request)
    {
        var permiso = new UsuarioPermiso
        {
            UsuarioId = request.UsuarioId,
            OpcionAccionId = request.OpcionAccionId,
            Permitido = request.Permitido
        };

        await _menuRepository.CrearUsuarioPermisoAsync(permiso);

        _logger.LogInformation(
            "Permiso {Permitido} asignado al usuario ID: {Id} para OpcionAccion ID: {OaId}",
            request.Permitido, request.UsuarioId, request.OpcionAccionId);
    }

    public async Task RevocarPermisoAsync(int usuarioId, int opcionAccionId)
    {
        await _menuRepository.EliminarUsuarioPermisoAsync(usuarioId, opcionAccionId);

        _logger.LogInformation(
            "Permiso revocado para usuario ID: {Id}, OpcionAccion ID: {OaId}",
            usuarioId, opcionAccionId);
    }
}
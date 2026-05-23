namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService) =>
        _menuService = menuService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuDto.MenuResponse>>>> ObtenerMiMenu()
    {
        var response = await _menuService.ObtenerMenuPorUsuarioAsync(UsuarioId, RolId);
        return HandleResponse(response);
    }

    [HttpPost("permisos")]
    [Authorize(Roles = "Administrador")] // Solo el admin real gestiona permisos
    public async Task<ActionResult<ApiResponse>> AsignarPermiso([FromBody] PermisoDto.AsignarRequest request)
    {
        await _menuService.AsignarPermisoAsync(request);
        return HandleSuccess("Permiso asignado exitosamente.");
    }

    [HttpDelete("permisos/{usuarioId:int}/{opcionAccionId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> RevocarPermiso(int usuarioId, int opcionAccionId)
    {
        await _menuService.RevocarPermisoAsync(usuarioId, opcionAccionId);
        return HandleSuccess("Permiso revocado exitosamente.");
    }

    [HttpGet("permisos/rol/{rolId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PermisoDto.ModuloPermisoResponse>>>> ObtenerPermisosRol(int rolId)
    {
        var response = await _menuService.ObtenerPermisosRolAsync(rolId);
        return HandleResponse(response);
    }

    [HttpPatch("permisos/rol/{rolId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> ActualizarPermisoRol(int rolId, [FromBody] PermisoDto.ActualizarPermisoRolRequest request)
    {
        await _menuService.ActualizarPermisoRolAsync(rolId, request.OpcionAccionId, request.Permitido);
        return HandleSuccess("Permiso actualizado.");
    }
}
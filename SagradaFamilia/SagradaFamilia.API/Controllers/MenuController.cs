namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService) =>
        _menuService = menuService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuResponse>>>> ObtenerMiMenu()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var rolId = int.Parse(User.FindFirstValue("rolId")!);

        var response = await _menuService.ObtenerMenuPorUsuarioAsync(usuarioId, rolId);
        return Ok(ApiResponse<IEnumerable<MenuResponse>>.Ok(response));
    }

    [HttpPost("permisos")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> AsignarPermiso(
        [FromBody] AsignarPermisoRequest request)
    {
        await _menuService.AsignarPermisoAsync(request);
        return Ok(ApiResponse.OkNoData("Permiso asignado exitosamente."));
    }

    [HttpDelete("permisos/{usuarioId:int}/{opcionAccionId:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> RevocarPermiso(
        int usuarioId, int opcionAccionId)
    {
        await _menuService.RevocarPermisoAsync(usuarioId, opcionAccionId);
        return Ok(ApiResponse.OkNoData("Permiso revocado exitosamente."));
    }
}
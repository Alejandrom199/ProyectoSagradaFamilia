namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService) => _usuarioService = usuarioService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioDto.ListResponse>>>> ObtenerTodos()
    {
        var response = await _usuarioService.ObtenerTodosAsync();
        return HandleResponse(response);
    }

    [HttpGet("paginado")]
    public async Task<ActionResult<PagedResponse<UsuarioDto.ListResponse>>> ObtenerPaginado(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] bool asc = true)
    {
        var result = await _usuarioService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto.DetailResponse>>> ObtenerPorId(int id)
    {
        var response = await _usuarioService.ObtenerPorIdAsync(id);
        return HandleResponse(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UsuarioDto.DetailResponse>>> CrearAdmin([FromBody] UsuarioDto.Create request)
    {
        var response = await _usuarioService.CrearAsync(request);
        return HandleResponse(response, "Usuario administrativo creado.");
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult<ApiResponse>> ActualizarEstado(int id, [FromBody] bool activo)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _usuarioService.ActualizarEstadoAsync(id, activo, currentUserId);
        return HandleSuccess("Estado del usuario actualizado.");
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UsuarioDto.DetailResponse>>> ObtenerPerfilActual()
    {
        var usuarioId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response = await _usuarioService.ObtenerPorIdAsync(usuarioId);
        return HandleResponse(response);
    }
}
namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Services;
using SagradaFamilia.Domain.Enums;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CitasController : BaseController
{
    private readonly ICitaService _citaService;
    private readonly INinoService _ninoService;

    public CitasController(ICitaService citaService, INinoService ninoService)
    {
        _citaService = citaService;
        _ninoService = ninoService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CitaDto.Response>>> ObtenerPorId(int id)
    {
        var response = await _citaService.ObtenerPorIdAsync(id);
        return HandleResponse(response);
    }

    [HttpGet("historial")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> Historial()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            return Unauthorized();

        var response = await _citaService.ObtenerHistorialPorMedicoAsync(usuarioId);
        return HandleResponse(response);
    }

    [HttpGet("hoy")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> MisCitasHoy()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            return Unauthorized();

        var response = await _citaService.ObtenerPorMedicoIdAsync(usuarioId, DateOnly.FromDateTime(DateTime.Now));
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico, Administrador")]
    public async Task<ActionResult<ApiResponse<CitaDto.Response>>> Crear([FromBody] CitaDto.Create request)
    {
        var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(usuarioIdClaim, out var usuarioId))
        {
            return Unauthorized();
        }

        var response = await _citaService.CrearAsync(request, usuarioId);
        return HandleResponse(response, "Cita programada exitosamente.");
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Medico, Administrador")]
    public async Task<ActionResult<ApiResponse>> CambiarEstado(int id, [FromBody] EstadoCita nuevoEstado)
    {
        await _citaService.ActualizarEstadoAsync(id, nuevoEstado);
        return HandleSuccess("Estado de la cita actualizado.");
    }

    [HttpGet("proximas")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> Proximas()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            return Unauthorized();

        var response = await _citaService.ObtenerProximasPorMedicoAsync(usuarioId);
        return HandleResponse(response);
    }

    [HttpGet("mis-hijos")]
    [Authorize(Roles = "Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> CitasMisHijos()
    {
        var response = await _citaService.ObtenerPorPadreIdAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("nino/{ninoId:int}/paginado")]
    [Authorize(Roles = "Medico,Padre,Administrador")]
    public async Task<ActionResult<PagedResponse<CitaDto.Response>>> PorNinoPaginado(
        int ninoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool asc = false)
    {
        if (User.IsInRole("Padre"))
        {
            var esSuHijo = await _ninoService.PerteneceAPadreAsync(ninoId, UsuarioId);
            if (!esSuHijo) return Forbid();
        }

        var (items, total) = await _citaService.ObtenerPaginadoPorNinoAsync(ninoId, page, pageSize, search, sortBy, asc);
        return PagedResponse<CitaDto.Response>.Ok(items, total, page, pageSize);
    }

    [HttpGet("nino/{ninoId:int}")]
    [Authorize(Roles = "Medico,Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> PorNino(int ninoId)
    {
        // Si es padre, validar que el niño le pertenece
        if (User.IsInRole("Padre"))
        {
            var esSuHijo = await _ninoService.PerteneceAPadreAsync(ninoId, UsuarioId);
            if (!esSuHijo) return Forbid();
        }

        var response = await _citaService.ObtenerPorNinoIdAsync(ninoId);
        return HandleResponse(response);
    }
}
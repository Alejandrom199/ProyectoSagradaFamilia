namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Common;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CitasController : BaseController
{
    private readonly ICitaService _citaService;
    private readonly INinoService _ninoService;
    private readonly IPadreService _padreService;
    private readonly IConsultaService _consultaService;

    public CitasController(ICitaService citaService, INinoService ninoService, IPadreService padreService, IConsultaService consultaService)
    {
        _citaService = citaService;
        _ninoService = ninoService;
        _padreService = padreService;
        _consultaService = consultaService;
    }

    // El PadreId (tabla Padres) es distinto del UsuarioId (claim del JWT) — hay que resolverlo primero.
    private async Task<int?> ObtenerPadreIdAsync()
    {
        var padre = await _padreService.ObtenerPorUsuarioIdAsync(UsuarioId);
        return padre?.Id;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CitaDto.Response>>> ObtenerPorId(int id)
    {
        var response = await _citaService.ObtenerPorIdAsync(id);

        if (User.IsInRole("Padre"))
        {
            var padreId = await ObtenerPadreIdAsync();
            if (padreId == null) return Forbid();

            var esSuHijo = await _ninoService.PerteneceAPadreAsync(response.NinoId, padreId.Value);
            if (!esSuHijo) return Forbid();
        }

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

    [HttpGet("historial/exportar")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> ExportarHistorialExcel()
    {
        var bytes = await _citaService.ExportarExcelPorMedicoAsync(UsuarioId);
        string filename = $"historial-citas-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("hoy")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> MisCitasHoy()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            return Unauthorized();

        var response = await _citaService.ObtenerPorMedicoIdAsync(usuarioId, DateOnly.FromDateTime(RelojEcuador.Ahora));
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico, Administrador")]
    public async Task<ActionResult<ApiResponse<CitaDto.Response>>> Crear([FromBody] CitaDto.Create request)
    {
        var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        var response = await _citaService.CrearAsync(request, usuarioId);
        return HandleResponse(response, "Cita programada exitosamente.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico, Administrador")]
    public async Task<ActionResult<ApiResponse<CitaDto.Response>>> Actualizar(int id, [FromBody] CitaDto.Update request)
    {
        var response = await _citaService.ActualizarAsync(id, request, UsuarioId);
        return HandleResponse(response, "Cita reagendada exitosamente.");
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Medico, Administrador")]
    public async Task<ActionResult<ApiResponse>> CambiarEstado(int id, [FromBody] CitaDto.CambiarEstadoRequest request)
    {
        await _citaService.ActualizarEstadoAsync(id, request);
        return HandleSuccess("Estado de la cita actualizado.");
    }

    [HttpPatch("{id:int}/iniciar-consulta")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<ConsultaDto.Response>>> IniciarConsulta(int id)
    {
        var response = await _consultaService.IniciarAsync(id, UsuarioId);
        return HandleResponse(response, "Consulta iniciada.");
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
        var padreId = await ObtenerPadreIdAsync();
        if (padreId == null) return Forbid();

        var response = await _citaService.ObtenerPorPadreIdAsync(padreId.Value);
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
            var padreId = await ObtenerPadreIdAsync();
            if (padreId == null) return Forbid();

            var esSuHijo = await _ninoService.PerteneceAPadreAsync(ninoId, padreId.Value);
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
            var padreId = await ObtenerPadreIdAsync();
            if (padreId == null) return Forbid();

            var esSuHijo = await _ninoService.PerteneceAPadreAsync(ninoId, padreId.Value);
            if (!esSuHijo) return Forbid();
        }

        var response = await _citaService.ObtenerPorNinoIdAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpGet("nino/{ninoId:int}/exportar")]
    [Authorize(Roles = "Medico,Administrador")]
    public async Task<IActionResult> ExportarPorNinoExcel(int ninoId)
    {
        var bytes = await _citaService.ExportarExcelPorNinoAsync(ninoId);
        string filename = $"citas-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }
}
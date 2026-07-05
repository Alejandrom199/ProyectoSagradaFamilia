namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PrescripcionesController : BaseController
{
    private readonly IPrescripcionService _prescripcionService;
    private readonly INinoService _ninoService;
    private readonly IPadreService _padreService;

    public PrescripcionesController(
        IPrescripcionService prescripcionService,
        INinoService ninoService,
        IPadreService padreService)
    {
        _prescripcionService = prescripcionService;
        _ninoService = ninoService;
        _padreService = padreService;
    }

    // El PadreId (tabla Padres) es distinto del UsuarioId (claim del JWT) — hay que resolverlo primero.
    private async Task<int?> ObtenerPadreIdAsync()
    {
        var padre = await _padreService.ObtenerPorUsuarioIdAsync(UsuarioId);
        return padre?.Id;
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<PrescripcionDto.Response>>> Crear([FromBody] PrescripcionDto.Create request)
    {
        // Pasamos el UsuarioId del token como el médico que prescribe
        var response = await _prescripcionService.CrearAsync(request, UsuarioId);
        return HandleResponse(response, "Prescripción registrada.");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PrescripcionDto.Response>>> ObtenerPorId(int id)
    {
        var response = await _prescripcionService.ObtenerPorIdAsync(id);

        if (User.IsInRole("Padre"))
        {
            var padreId = await ObtenerPadreIdAsync();
            if (padreId == null) return Forbid();

            var esSuHijo = await _ninoService.PerteneceAPadreAsync(response.NinoId, padreId.Value);
            if (!esSuHijo) return Forbid();
        }

        return HandleResponse(response);
    }

    [HttpGet("mis-prescripciones")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PrescripcionDto.Response>>>> MisPrescripciones()
    {
        var response = await _prescripcionService.ObtenerPorMedicoAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("mis-prescripciones/exportar")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> ExportarMisPrescripcionesExcel()
    {
        var bytes = await _prescripcionService.ExportarExcelPorMedicoAsync(UsuarioId);
        string filename = $"mis-prescripciones-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("nino/{ninoId:int}/paginado")]
    [Authorize(Roles = "Medico,Padre,Administrador")]
    public async Task<ActionResult<PagedResponse<PrescripcionDto.Response>>> HistorialNinoPaginado(
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

        var (items, total) = await _prescripcionService.ObtenerPaginadoPorNinoAsync(ninoId, page, pageSize, search, sortBy, asc);
        return PagedResponse<PrescripcionDto.Response>.Ok(items, total, page, pageSize);
    }

    [HttpGet("nino/{ninoId:int}")]
    [Authorize(Roles = "Medico,Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PrescripcionDto.Response>>>> HistorialNino(int ninoId)
    {
        if (User.IsInRole("Padre"))
        {
            var padreId = await ObtenerPadreIdAsync();
            if (padreId == null) return Forbid();

            var esSuHijo = await _ninoService.PerteneceAPadreAsync(ninoId, padreId.Value);
            if (!esSuHijo) return Forbid();
        }

        var response = await _prescripcionService.ObtenerHistorialPorNinoAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpGet("nino/{ninoId:int}/exportar")]
    [Authorize(Roles = "Medico,Administrador")]
    public async Task<IActionResult> ExportarPorNinoExcel(int ninoId)
    {
        var bytes = await _prescripcionService.ExportarExcelPorNinoAsync(ninoId);
        string filename = $"prescripciones-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }
}
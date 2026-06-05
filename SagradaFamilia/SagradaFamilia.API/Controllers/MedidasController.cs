namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MedidasController : BaseController
{
    private readonly IMedidaService _medidaService;

    public MedidasController(IMedidaService medidaService) =>
        _medidaService = medidaService;

    [HttpGet("nino/{ninoId:int}/paginado")]
    public async Task<ActionResult<PagedResponse<MedidaDto.Response>>> ObtenerPorNinoPaginado(
        int ninoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool asc = false)
    {
        var (items, total) = await _medidaService.ObtenerPaginadoPorNinoAsync(ninoId, page, pageSize, search, sortBy, asc);
        return PagedResponse<MedidaDto.Response>.Ok(items, total, page, pageSize);
    }

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MedidaDto.Response>>>> ObtenerPorNino(int ninoId)
    {
        var response = await _medidaService.ObtenerPorNinoAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaDto.Response>>> Crear([FromBody] MedidaDto.Create request)
    {
        var response = await _medidaService.CrearAsync(request, MedicoId);

        return CreatedAtAction(nameof(ObtenerPorNino), new { ninoId = request.NinoId },
            ApiResponse<MedidaDto.Response>.Ok(response, "Medida registrada exitosamente."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaDto.Response>>> Actualizar(int id, [FromBody] MedidaDto.Update request)
    {
        var response = await _medidaService.ActualizarAsync(id, request);
        return HandleResponse(response, "Medida actualizada exitosamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _medidaService.EliminarAsync(id);
        return HandleSuccess("Medida eliminada exitosamente.");
    }

    [HttpGet("exportar")]
    [Authorize(Roles = "Administrador,Medico")]
    public async Task<IActionResult> ExportarExcel()
    {
        var bytes = await _medidaService.ExportarExcelAsync();
        string filename = $"medidas-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("plantilla")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        var bytes = await _medidaService.GenerarPlantillaAsync();
        string filename = $"plantilla-medidas-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpPost("importar")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaDto.ImportResultado>>> Importar([FromForm] ImportarArchivoRequest request)
    {
        var archivo = request.Archivo;
        if (archivo is null || archivo.Length == 0)
            return BadRequest(ApiResponse<MedidaDto.ImportResultado>.Fail("No se recibió ningún archivo."));

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            return BadRequest(ApiResponse<MedidaDto.ImportResultado>.Fail("Solo se aceptan archivos .xlsx"));

        using var stream = archivo.OpenReadStream();
        var resultado = await _medidaService.ImportarAsync(stream, MedicoId);

        string mensaje = resultado.Errores.Count == 0
            ? $"{resultado.Importados} creadas, {resultado.Actualizados} actualizadas correctamente."
            : $"{resultado.Importados} creadas, {resultado.Actualizados} actualizadas, {resultado.Errores.Count} con errores.";

        return HandleResponse(resultado, mensaje);
    }
}
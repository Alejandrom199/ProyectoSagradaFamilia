using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MedicoController : BaseController
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService) => _medicoService = medicoService;

    [HttpGet]
    [Authorize(Roles = "Administrador, Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MedicoDto.ListResponse>>>> GetAll()
    {
        var result = await _medicoService.ObtenerTodosAsync();
        return HandleResponse(result);
    }

    [HttpGet("paginado")]
    [Authorize(Roles = "Administrador, Medico")]
    public async Task<ActionResult<PagedResponse<MedicoDto.ListResponse>>> GetPaginado(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] bool asc = true)
    {
        var result = await _medicoService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<MedicoDto.DetailResponse>>> ObtenerPorId(int id)
    {
        var result = await _medicoService.ObtenerPorIdAsync(id);
        return HandleResponse(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<MedicoDto.DetailResponse>>> Create([FromBody] MedicoDto.Create request)
    {
        var result = await _medicoService.CrearAsync(request);
        return HandleResponse(result, "Médico registrado correctamente.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<MedicoDto.DetailResponse>>> Actualizar(int id, [FromBody] MedicoDto.Update request)
    {
        var result = await _medicoService.ActualizarAsync(id, request);
        return HandleResponse(result, "Médico actualizado correctamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _medicoService.EliminarAsync(id);
        return HandleSuccess("Médico eliminado.");
    }

    [HttpPost("{id:int}/reset-password")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> ResetPassword(int id)
    {
        await _medicoService.RestablecerPasswordAsync(id);
        return HandleSuccess("Se ha enviado un enlace de restablecimiento al correo del médico.");
    }

    [HttpGet("exportar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ExportarExcel()
    {
        var bytes = await _medicoService.ExportarExcelAsync();
        string filename = $"medicos-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("plantilla")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        var bytes = await _medicoService.GenerarPlantillaAsync();
        string filename = $"plantilla-medicos-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpPost("importar")]
    [Authorize(Roles = "Administrador")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<MedicoDto.ImportResultado>>> Importar([FromForm] ImportarArchivoRequest request)
    {
        var archivo = request.Archivo;
        if (archivo is null || archivo.Length == 0)
            return BadRequest(ApiResponse<MedicoDto.ImportResultado>.Fail("No se recibió ningún archivo."));

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            return BadRequest(ApiResponse<MedicoDto.ImportResultado>.Fail("Solo se aceptan archivos .xlsx"));

        using var stream = archivo.OpenReadStream();
        var resultado = await _medicoService.ImportarAsync(stream);

        string mensaje = resultado.Errores.Count == 0
            ? $"{resultado.Importados} creados, {resultado.Actualizados} actualizados correctamente."
            : $"{resultado.Importados} creados, {resultado.Actualizados} actualizados, {resultado.Errores.Count} con errores.";

        return HandleResponse(resultado, mensaje);
    }

    [HttpGet("mi-firma")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<string?>>> ObtenerMiFirma()
    {
        var firma = await _medicoService.ObtenerFirmaAsync(MedicoId);
        return HandleResponse(firma);
    }

    [HttpPut("mi-firma")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> ActualizarMiFirma([FromBody] MedicoDto.ActualizarFirmaRequest request)
    {
        await _medicoService.ActualizarFirmaAsync(MedicoId, request.ImagenBase64);
        return HandleSuccess("Firma guardada correctamente.");
    }

    [HttpDelete("mi-firma")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> EliminarMiFirma()
    {
        await _medicoService.EliminarFirmaAsync(MedicoId);
        return HandleSuccess("Firma eliminada.");
    }
}
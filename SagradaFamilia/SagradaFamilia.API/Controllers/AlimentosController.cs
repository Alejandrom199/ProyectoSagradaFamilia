namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlimentosController : BaseController
{
    private readonly IAlimentoService _alimentoService;

    public AlimentosController(IAlimentoService alimentoService) =>
        _alimentoService = alimentoService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<AlimentoDto.Response>>>> ObtenerTodos()
    {
        var response = await _alimentoService.ObtenerTodosAsync();
        return HandleResponse(response);
    }

    [HttpGet("paginado")]
    public async Task<ActionResult<PagedResponse<AlimentoDto.Response>>> ObtenerPaginado(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool asc = true)
    {
        var response = await _alimentoService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
        return Ok(response);
    }

    [HttpGet("por-edad/{edadMeses:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AlimentoDto.Response>>>> ObtenerPorEdad(int edadMeses)
    {
        var response = await _alimentoService.ObtenerPorRangoEdadAsync(edadMeses);
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<AlimentoDto.Response>>> Crear([FromBody] AlimentoDto.Create request)
    {
        var response = await _alimentoService.CrearAsync(request);
        return HandleResponse(response, "Alimento creado exitosamente.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<AlimentoDto.Response>>> Actualizar(int id, [FromBody] AlimentoDto.Update request)
    {
        var response = await _alimentoService.ActualizarAsync(id, request);
        return HandleResponse(response, "Alimento actualizado.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _alimentoService.EliminarAsync(id);
        return HandleSuccess("Alimento eliminado.");
    }

    [HttpGet("categorias")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaDto.Response>>>> ObtenerCategorias()
    {
        var response = await _alimentoService.ObtenerCategoriasAsync();
        return HandleResponse(response);
    }

    [HttpPost("categorias")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<CategoriaDto.Response>>> CrearCategoria([FromBody] CategoriaDto.Create request)
    {
        var response = await _alimentoService.CrearCategoriaAsync(request);
        return HandleResponse(response, "Categoría creada.");
    }

    [HttpGet("exportar")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> ExportarExcel()
    {
        var bytes = await _alimentoService.ExportarExcelAsync();
        string filename = $"alimentos-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("plantilla")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        var bytes = await _alimentoService.GenerarPlantillaAsync();
        string filename = $"plantilla-alimentos-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpPost("importar")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<AlimentoDto.ImportResultado>>> Importar([FromForm] IFormFile archivo)
    {
        if (archivo is null || archivo.Length == 0)
            return BadRequest(ApiResponse<AlimentoDto.ImportResultado>.Fail("No se recibió ningún archivo."));

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            return BadRequest(ApiResponse<AlimentoDto.ImportResultado>.Fail("Solo se aceptan archivos .xlsx"));

        using var stream = archivo.OpenReadStream();
        var resultado = await _alimentoService.ImportarAsync(stream);

        string mensaje = resultado.Errores.Count == 0
            ? $"{resultado.Importados} creados, {resultado.Actualizados} actualizados correctamente."
            : $"{resultado.Importados} creados, {resultado.Actualizados} actualizados, {resultado.Errores.Count} con errores.";

        return HandleResponse(resultado, mensaje);
    }
}
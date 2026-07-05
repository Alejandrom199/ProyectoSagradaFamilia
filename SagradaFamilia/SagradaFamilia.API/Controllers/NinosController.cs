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
public class NinosController : BaseController
{
    private readonly INinoService _ninoService;
    private readonly IPadreService _padreService;

    public NinosController(
        INinoService ninoService,
        IPadreService padreService
    )
    {
        _ninoService = ninoService;
        _padreService = padreService;
    }
        

    [HttpGet("paginado")]
    [Authorize(Roles = "Medico,Administrador")]
    public async Task<ActionResult<PagedResponse<NinoDto.ListResponse>>> ObtenerPaginado(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] bool asc = true)
    {
        int? medicoId = User.IsInRole("Medico") ? MedicoId : null;
        var result = await _ninoService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc, medicoId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerTodos()
    {
        var response = await _ninoService.ObtenerMisPacientesPorUsuarioIdAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("todos")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerTodosAdmin()
    {
        var response = await _ninoService.ObtenerTodosAsync();
        return HandleResponse(response);
    }

    [HttpGet("padre/{padreId:int}")]
    [Authorize(Roles = "Medico,Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerPorPadre(int padreId)
    {
        var response = await _ninoService.ObtenerPorPadreIdAsync(padreId);
        return HandleResponse(response);
    }

    [HttpGet("mis-ninos")]
    [Authorize(Roles = "Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerMisNinos()
    {
        var response = await _ninoService.ObtenerMisPorUsuarioIdAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Medico,Padre")]
    public async Task<ActionResult<ApiResponse<NinoDto.DetailResponse>>> ObtenerPorId(int id)
    {
        if (User.IsInRole("Padre"))
        {
            var padre = await _padreService.ObtenerPorUsuarioIdAsync(UsuarioId);
            if (padre == null) return Forbid();

            var esSuHijo = await _ninoService.PerteneceAPadreAsync(id, padre.Id);
            if (!esSuHijo) return Forbid();
        }

        var response = await _ninoService.ObtenerPorIdAsync(id);
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<NinoDto.DetailResponse>>> Crear([FromBody] NinoDto.Create request)
    {
        var response = await _ninoService.CrearAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = response.Id },
            ApiResponse<NinoDto.DetailResponse>.Ok(response, "Niño registrado exitosamente."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico,Administrador")]
    public async Task<ActionResult<ApiResponse<NinoDto.DetailResponse>>> Actualizar(int id, [FromBody] NinoDto.Update request)
    {
        var response = await _ninoService.ActualizarAsync(id, request);
        return HandleResponse(response, "Datos del niño actualizados.");
    }

    [HttpPatch("{id:int}/medico")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> CambiarMedico(int id, [FromBody] NinoDto.ChangeMedico request)
    {
        await _ninoService.CambiarMedicoAsync(id, request.MedicoId);
        return HandleSuccess("Médico reasignado correctamente.");
    }

    [HttpPatch("{id:int}/padre")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> CambiarPadre(int id, [FromBody] NinoDto.ChangePadre request)
    {
        await _ninoService.CambiarPadreAsync(id, request.PadreId);
        return HandleSuccess("Representante reasignado correctamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _ninoService.EliminarAsync(id);
        return HandleSuccess("Niño eliminado del sistema.");
    }

    [HttpGet("exportar")]
    [Authorize(Roles = "Administrador,Medico")]
    public async Task<IActionResult> ExportarExcel()
    {
        var bytes = await _ninoService.ExportarExcelAsync();
        string filename = $"pacientes-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("plantilla")]
    [Authorize(Roles = "Administrador,Medico")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        var bytes = await _ninoService.GenerarPlantillaAsync();
        string filename = $"plantilla-pacientes-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpPost("importar")]
    [Authorize(Roles = "Medico")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<NinoDto.ImportResultado>>> Importar([FromForm] ImportarArchivoRequest request)
    {
        var archivo = request.Archivo;
        if (archivo is null || archivo.Length == 0)
            return BadRequest(ApiResponse<NinoDto.ImportResultado>.Fail("No se recibió ningún archivo."));

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            return BadRequest(ApiResponse<NinoDto.ImportResultado>.Fail("Solo se aceptan archivos .xlsx"));

        using var stream = archivo.OpenReadStream();
        var resultado = await _ninoService.ImportarAsync(stream, MedicoId);

        string mensaje = resultado.Errores.Count == 0
            ? $"{resultado.Importados} creados, {resultado.Actualizados} actualizados correctamente."
            : $"{resultado.Importados} creados, {resultado.Actualizados} actualizados, {resultado.Errores.Count} con errores.";

        return HandleResponse(resultado, mensaje);
    }
}
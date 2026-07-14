namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService) => _usuarioService = usuarioService;

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioDto.ListResponse>>>> ObtenerTodos()
    {
        var response = await _usuarioService.ObtenerTodosAsync();
        return HandleResponse(response);
    }

    [HttpGet("paginado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<PagedResponse<UsuarioDto.ListResponse>>> ObtenerPaginado(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] bool asc = true)
    {
        var result = await _usuarioService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<UsuarioDto.DetailResponse>>> ObtenerPorId(int id)
    {
        var response = await _usuarioService.ObtenerPorIdAsync(id);
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<UsuarioDto.DetailResponse>>> CrearAdmin([FromBody] UsuarioDto.Create request)
    {
        var response = await _usuarioService.CrearAsync(request);
        return HandleResponse(response, "Usuario administrativo creado.");
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> ActualizarEstado(int id, [FromBody] UsuarioDto.ActualizarEstadoRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _usuarioService.ActualizarEstadoAsync(id, request.Activo, request.Motivo, currentUserId);
        return HandleSuccess("Estado del usuario actualizado.");
    }

    [HttpGet("{id:int}/historial-estado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioDto.EstadoHistorialResponse>>>> ObtenerHistorialEstado(int id)
    {
        var historial = await _usuarioService.ObtenerHistorialEstadoAsync(id);
        return HandleResponse(historial);
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UsuarioDto.DetailResponse>>> ObtenerPerfilActual()
    {
        var usuarioId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response = await _usuarioService.ObtenerPorIdAsync(usuarioId);
        return HandleResponse(response);
    }

    [HttpGet("exportar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ExportarExcel()
    {
        var bytes = await _usuarioService.ExportarExcelAsync();
        string filename = $"usuarios-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("plantilla")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        var bytes = await _usuarioService.GenerarPlantillaAsync();
        string filename = $"plantilla-usuarios-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpPost("importar")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<UsuarioDto.ImportResultado>>> Importar([FromForm] ImportarArchivoRequest request)
    {
        var archivo = request.Archivo;
        if (archivo is null || archivo.Length == 0)
            return BadRequest(ApiResponse<UsuarioDto.ImportResultado>.Fail("No se recibió ningún archivo."));

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            return BadRequest(ApiResponse<UsuarioDto.ImportResultado>.Fail("Solo se aceptan archivos .xlsx"));

        using var stream = archivo.OpenReadStream();
        var resultado = await _usuarioService.ImportarAsync(stream);

        string mensaje = resultado.Errores.Count == 0
            ? $"{resultado.Importados} creados, {resultado.Actualizados} actualizados correctamente."
            : $"{resultado.Importados} creados, {resultado.Actualizados} actualizados, {resultado.Errores.Count} con errores.";

        return HandleResponse(resultado, mensaje);
    }
}
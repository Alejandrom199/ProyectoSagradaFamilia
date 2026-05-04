namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Alimentos;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlimentosController : ControllerBase
{
    private readonly IAlimentoService _alimentoService;

    public AlimentosController(IAlimentoService alimentoService) =>
        _alimentoService = alimentoService;

    [HttpGet]
    [Authorize(Roles = "Medico, Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AlimentoResponse>>>> ObtenerTodos()
    {
        var response = await _alimentoService.ObtenerTodosAsync();
        return Ok(ApiResponse<IEnumerable<AlimentoResponse>>.Ok(response));
    }

    [HttpGet("por-edad/{edadMeses:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AlimentoResponse>>>> ObtenerPorEdad(
        int edadMeses)
    {
        var response = await _alimentoService.ObtenerPorEdadAsync(edadMeses);
        return Ok(ApiResponse<IEnumerable<AlimentoResponse>>.Ok(response));
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<AlimentoResponse>>> Crear(
        [FromBody] CrearAlimentoRequest request)
    {
        var response = await _alimentoService.CrearAsync(request);
        return CreatedAtAction(nameof(ObtenerTodos),
            ApiResponse<AlimentoResponse>.Ok(response, "Alimento creado exitosamente."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<AlimentoResponse>>> Actualizar(
        int id, [FromBody] CrearAlimentoRequest request)
    {
        var response = await _alimentoService.ActualizarAsync(id, request);
        return Ok(ApiResponse<AlimentoResponse>.Ok(response, "Alimento actualizado exitosamente."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _alimentoService.EliminarAsync(id);
        return Ok(ApiResponse.OkNoData("Alimento eliminado exitosamente."));
    }

    [HttpGet("categorias")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaResponse>>>> ObtenerCategorias()
    {
        var response = await _alimentoService.ObtenerCategoriasAsync();
        return Ok(ApiResponse<IEnumerable<CategoriaResponse>>.Ok(response));
    }

    [HttpPost("categorias")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<CategoriaResponse>>> CrearCategoria(
        [FromBody] CrearCategoriaRequest request)
    {
        var response = await _alimentoService.CrearCategoriaAsync(request);
        return CreatedAtAction(nameof(ObtenerCategorias),
            ApiResponse<CategoriaResponse>.Ok(response, "Categoría creada exitosamente."));
    }
}
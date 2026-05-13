namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
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

    [HttpGet("por-edad/{edadMeses:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AlimentoDto.Response>>>> ObtenerPorEdad(int edadMeses)
    {
        // 💡 Corregido: Nombre exacto de la interfaz IAlimentoService
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
}
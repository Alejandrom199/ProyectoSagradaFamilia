namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ParametrosController : BaseController
{
    private readonly IParametroService _parametroService;

    public ParametrosController(IParametroService parametroService) =>
        _parametroService = parametroService;

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParametroDto.Response>>>> ObtenerTodos()
    {
        var data = await _parametroService.ObtenerTodosAsync();
        return HandleResponse(data);
    }

    [HttpGet("grupo/{grupo}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParametroDto.Response>>>> ObtenerPorGrupo(string grupo)
    {
        var data = await _parametroService.ObtenerPorGrupoAsync(grupo);
        return HandleResponse(data);
    }

    [HttpGet("grupo/{grupo}/codigo/{codigo}")]
    public async Task<ActionResult<ApiResponse<ParametroDto.Response?>>> ObtenerPorGrupoYCodigo(string grupo, string codigo)
    {
        var data = await _parametroService.ObtenerPorGrupoYCodigoAsync(grupo, codigo);
        return HandleResponse(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ParametroDto.Response>>> ObtenerPorId(int id)
    {
        var data = await _parametroService.ObtenerPorIdAsync(id);
        return HandleResponse(data);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<ParametroDto.Response>>> Crear([FromBody] ParametroDto.Create request)
    {
        var data = await _parametroService.CrearAsync(request);
        return HandleResponse(data, "Parámetro creado exitosamente.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<ParametroDto.Response>>> Actualizar(int id, [FromBody] ParametroDto.Update request)
    {
        var data = await _parametroService.ActualizarAsync(id, request);
        return HandleResponse(data, "Parámetro actualizado exitosamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _parametroService.EliminarAsync(id);
        return HandleSuccess("Parámetro eliminado exitosamente.");
    }
}

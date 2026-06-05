namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class PlantillasController : BaseController
{
    private readonly IPlantillaCorreoService _plantillaService;

    public PlantillasController(IPlantillaCorreoService plantillaService) =>
        _plantillaService = plantillaService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PlantillaCorreoDto.Response>>>> ObtenerTodos()
    {
        var data = await _plantillaService.ObtenerTodosAsync();
        return HandleResponse(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PlantillaCorreoDto.Response>>> ObtenerPorId(int id)
    {
        var data = await _plantillaService.ObtenerPorIdAsync(id);
        return HandleResponse(data);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PlantillaCorreoDto.Response>>> Crear([FromBody] PlantillaCorreoDto.Create request)
    {
        var data = await _plantillaService.CrearAsync(request);
        return HandleResponse(data, "Plantilla creada exitosamente.");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PlantillaCorreoDto.Response>>> Actualizar(int id, [FromBody] PlantillaCorreoDto.Update request)
    {
        var data = await _plantillaService.ActualizarAsync(id, request);
        return HandleResponse(data, "Plantilla actualizada exitosamente.");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _plantillaService.EliminarAsync(id);
        return HandleSuccess("Plantilla eliminada exitosamente.");
    }
}

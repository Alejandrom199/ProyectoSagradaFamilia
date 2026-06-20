namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class EventosCorreoController : BaseController
{
    private readonly IEventoCorreoService _eventoService;

    public EventosCorreoController(IEventoCorreoService eventoService) =>
        _eventoService = eventoService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EventoCorreoDto.Response>>>> ObtenerTodos()
    {
        var data = await _eventoService.ObtenerTodosAsync();
        return HandleResponse(data);
    }

    [HttpPatch("{id:int}/plantilla")]
    public async Task<ActionResult<ApiResponse<EventoCorreoDto.Response>>> AsignarPlantilla(
        int id, [FromBody] EventoCorreoDto.AsignarPlantilla request)
    {
        var data = await _eventoService.AsignarPlantillaAsync(id, request.PlantillaCorreoId);
        return HandleResponse(data, "Plantilla asignada correctamente.");
    }
}

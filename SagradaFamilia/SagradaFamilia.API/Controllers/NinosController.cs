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
        

    [HttpGet]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerTodos()
    {
        var response = await _ninoService.ObtenerPorMedicoIdAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("todos")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerTodosAdmin()
    {
        var response = await _ninoService.ObtenerTodosAsync();
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
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<NinoDto.DetailResponse>>> Actualizar(int id, [FromBody] NinoDto.Update request)
    {
        var response = await _ninoService.ActualizarAsync(id, request);
        return HandleResponse(response, "Datos del niño actualizados.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _ninoService.EliminarAsync(id);
        return HandleSuccess("Niño eliminado del sistema.");
    }
}
namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
public class NinosController : BaseController
{
    private readonly INinoService _ninoService;

    public NinosController(INinoService ninoService) =>
        _ninoService = ninoService;

    [HttpGet]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerTodos()
    {
        var response = await _ninoService.ObtenerTodosAsync();
        return HandleResponse(response);
    }

    [HttpGet("mis-ninos")]
    [Authorize(Roles = "Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoDto.ListResponse>>>> ObtenerMisNinos()
    {
        var response = await _ninoService.ObtenerPorPadreIdAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<NinoDto.DetailResponse>>> ObtenerPorId(int id)
    {
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
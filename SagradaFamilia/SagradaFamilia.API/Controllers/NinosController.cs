namespace SagradaFamilia.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.DTOs.Ninos;
using SagradaFamilia.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NinosController : ControllerBase
{
    private readonly INinoService _ninoService;

    public NinosController(INinoService ninoService) =>
        _ninoService = ninoService;

    [HttpGet]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoResponse>>>> ObtenerTodos()
    {
        var response = await _ninoService.ObtenerTodosAsync();
        return Ok(ApiResponse<IEnumerable<NinoResponse>>.Ok(response));
    }

    [HttpGet("mis-ninos")]
    [Authorize(Roles = "Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NinoResponse>>>> ObtenerMisNinos()
    {
        var representanteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _ninoService.ObtenerPorRepresentanteAsync(representanteId);
        return Ok(ApiResponse<IEnumerable<NinoResponse>>.Ok(response));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<NinoResponse>>> ObtenerPorId(int id)
    {
        var response = await _ninoService.ObtenerPorIdAsync(id);
        return Ok(ApiResponse<NinoResponse>.Ok(response));
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<NinoResponse>>> Crear(
        [FromBody] CrearNinoRequest request)
    {
        var response = await _ninoService.CrearAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId),
            new { id = response.Id },
            ApiResponse<NinoResponse>.Ok(response, "Niño registrado exitosamente."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<NinoResponse>>> Actualizar(
        int id, [FromBody] ActualizarNinoRequest request)
    {
        var response = await _ninoService.ActualizarAsync(id, request);
        return Ok(ApiResponse<NinoResponse>.Ok(response, "Niño actualizado exitosamente."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _ninoService.EliminarAsync(id);
        return Ok(ApiResponse.OkNoData("Niño eliminado exitosamente."));
    }
}
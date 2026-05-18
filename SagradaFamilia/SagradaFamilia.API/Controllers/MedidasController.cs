namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MedidasController : BaseController
{
    private readonly IMedidaService _medidaService;

    public MedidasController(IMedidaService medidaService) =>
        _medidaService = medidaService;

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MedidaDto.Response>>>> ObtenerPorNino(int ninoId)
    {
        var response = await _medidaService.ObtenerPorNinoAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaDto.Response>>> Crear([FromBody] MedidaDto.Create request)
    {
        var response = await _medidaService.CrearAsync(request, UsuarioId);

        return CreatedAtAction(nameof(ObtenerPorNino), new { ninoId = request.NinoId },
            ApiResponse<MedidaDto.Response>.Ok(response, "Medida registrada exitosamente."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaDto.Response>>> Actualizar(int id, [FromBody] MedidaDto.Update request)
    {
        var response = await _medidaService.ActualizarAsync(id, request);
        return HandleResponse(response, "Medida actualizada exitosamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _medidaService.EliminarAsync(id);
        return HandleSuccess("Medida eliminada exitosamente.");
    }
}
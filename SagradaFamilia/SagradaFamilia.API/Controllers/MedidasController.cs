namespace SagradaFamilia.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.DTOs.Medidas;
using SagradaFamilia.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedidasController : ControllerBase
{
    private readonly IMedidaService _medidaService;

    public MedidasController(IMedidaService medidaService) =>
        _medidaService = medidaService;

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MedidaResponse>>>> ObtenerPorNino(
        int ninoId)
    {
        var response = await _medidaService.ObtenerPorNinoAsync(ninoId);
        return Ok(ApiResponse<IEnumerable<MedidaResponse>>.Ok(response));
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaResponse>>> Crear(
        [FromBody] CrearMedidaRequest request)
    {
        var medicoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _medidaService.CrearAsync(request, medicoId);
        return CreatedAtAction(nameof(ObtenerPorNino),
            new { ninoId = request.NinoId },
            ApiResponse<MedidaResponse>.Ok(response, "Medida registrada exitosamente."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<MedidaResponse>>> Actualizar(
        int id, [FromBody] ActualizarMedidaRequest request)
    {
        var response = await _medidaService.ActualizarAsync(id, request);
        return Ok(ApiResponse<MedidaResponse>.Ok(response, "Medida actualizada exitosamente."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _medidaService.EliminarAsync(id);
        return Ok(ApiResponse.OkNoData("Medida eliminada exitosamente."));
    }
}
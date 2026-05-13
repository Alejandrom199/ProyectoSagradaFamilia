namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
public class PrescripcionesController : BaseController
{
    private readonly IPrescripcionService _prescripcionService;

    public PrescripcionesController(IPrescripcionService prescripcionService) =>
        _prescripcionService = prescripcionService;

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PrescripcionDto.Response>>>> HistorialNino(int ninoId)
    {
        var response = await _prescripcionService.ObtenerHistorialPorNinoAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<PrescripcionDto.Response>>> Crear([FromBody] PrescripcionDto.Create request)
    {
        // Pasamos el UsuarioId del token como el médico que prescribe
        var response = await _prescripcionService.CrearAsync(request, UsuarioId);
        return HandleResponse(response, "Prescripción registrada.");
    }
}
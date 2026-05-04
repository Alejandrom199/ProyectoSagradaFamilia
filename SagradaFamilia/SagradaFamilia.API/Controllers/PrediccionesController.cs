namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.DTOs.Predicciones;
using SagradaFamilia.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrediccionesController : ControllerBase
{
    private readonly IPrediccionService _prediccionService;

    public PrediccionesController(IPrediccionService prediccionService) =>
        _prediccionService = prediccionService;

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<PrediccionResponse>>> ObtenerPredicciones(
        int ninoId)
    {
        var response = await _prediccionService.ObtenerPrediccionesAsync(ninoId);
        return Ok(ApiResponse<PrediccionResponse>.Ok(response));
    }

    [HttpGet("health")]
    public async Task<ActionResult<ApiResponse<PrediccionHealth>>> ObtenerEstadoServicioPrediccion()
    {
        var response = await _prediccionService.ObtenerEstadoServicioPrediccionAsync();
        return Ok(ApiResponse<PrediccionHealth>.Ok(response));
    }

}
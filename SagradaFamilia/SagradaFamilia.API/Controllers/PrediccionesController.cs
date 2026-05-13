using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers;

public class PrediccionesController : BaseController
{
    private readonly IPrediccionService _prediccionService;

    public PrediccionesController(IPrediccionService prediccionService) =>
        _prediccionService = prediccionService;

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<PrediccionDto.Response>>> ObtenerPredicciones(int ninoId)
    {
        var response = await _prediccionService.ObtenerPrediccionesAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpGet("health")]
    public async Task<ActionResult<ApiResponse<PrediccionDto.Health>>> ObtenerEstadoServicioPrediccion()
    {
        var response = await _prediccionService.ObtenerEstadoServicioPrediccionAsync();
        return HandleResponse(response);
    }
}  
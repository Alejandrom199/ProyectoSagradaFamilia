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
public class PrescripcionesController : BaseController
{
    private readonly IPrescripcionService _prescripcionService;
    private readonly INinoService _ninoService;

    public PrescripcionesController(
        IPrescripcionService prescripcionService, 
        INinoService ninoService)
    {
        _prescripcionService = prescripcionService;
        _ninoService = ninoService;
    }

    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<PrescripcionDto.Response>>> Crear([FromBody] PrescripcionDto.Create request)
    {
        // Pasamos el UsuarioId del token como el médico que prescribe
        var response = await _prescripcionService.CrearAsync(request, UsuarioId);
        return HandleResponse(response, "Prescripción registrada.");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PrescripcionDto.Response>>> ObtenerPorId(int id)
    {
        var response = await _prescripcionService.ObtenerPorIdAsync(id);
        return HandleResponse(response);
    }

    [HttpGet("mis-prescripciones")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PrescripcionDto.Response>>>> MisPrescripciones()
    {
        var response = await _prescripcionService.ObtenerPorMedicoAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("nino/{ninoId:int}")]
    [Authorize(Roles = "Medico,Padre")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PrescripcionDto.Response>>>> HistorialNino(int ninoId)
    {
        if (User.IsInRole("Padre"))
        {
            var esSuHijo = await _ninoService.PerteneceAPadreAsync(ninoId, UsuarioId);
            if (!esSuHijo) return Forbid();
        }

        var response = await _prescripcionService.ObtenerHistorialPorNinoAsync(ninoId);
        return HandleResponse(response);
    }
}
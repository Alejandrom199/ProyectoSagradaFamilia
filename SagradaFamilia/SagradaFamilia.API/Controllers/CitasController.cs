namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Enums;

[Authorize]
public class CitasController : BaseController
{
    private readonly ICitaService _citaService;

    public CitasController(ICitaService citaService) => _citaService = citaService;

    [HttpGet("nino/{ninoId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> PorNino(int ninoId)
    {
        var response = await _citaService.ObtenerPorNinoIdAsync(ninoId);
        return HandleResponse(response);
    }

    [HttpGet("hoy")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CitaDto.Response>>>> MisCitasHoy()
    {
        // Usamos el UsuarioId del token para buscar el perfil médico o pasarlo al service
        var response = await _citaService.ObtenerPorMedicoIdAsync(UsuarioId, DateOnly.FromDateTime(DateTime.Now));
        return HandleResponse(response);
    }

    [HttpPost]
    [Authorize(Roles = "Medico, Administrador")]
    public async Task<ActionResult<ApiResponse<CitaDto.Response>>> Crear([FromBody] CitaDto.Create request)
    {
        var response = await _citaService.CrearAsync(request);
        return HandleResponse(response, "Cita programada exitosamente.");
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult<ApiResponse>> CambiarEstado(int id, [FromBody] EstadoCita nuevoEstado)
    {
        await _citaService.ActualizarEstadoAsync(id, nuevoEstado);
        return HandleSuccess("Estado de la cita actualizado.");
    }
}
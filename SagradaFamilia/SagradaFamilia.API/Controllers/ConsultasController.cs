namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConsultasController : BaseController
{
    private readonly IConsultaService _consultaService;

    public ConsultasController(IConsultaService consultaService)
    {
        _consultaService = consultaService;
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<ConsultaDto.Response>>> Actualizar(int id, [FromBody] ConsultaDto.Actualizar request)
    {
        var response = await _consultaService.ActualizarAsync(id, request, UsuarioId);
        return HandleResponse(response, "Consulta actualizada.");
    }

    [HttpPatch("{id:int}/completar")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<ConsultaDto.Response>>> Completar(int id)
    {
        var response = await _consultaService.CompletarAsync(id, UsuarioId);
        return HandleResponse(response, "Consulta completada.");
    }
}

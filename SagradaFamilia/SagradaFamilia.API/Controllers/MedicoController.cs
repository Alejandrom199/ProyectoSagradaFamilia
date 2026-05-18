using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MedicoController : BaseController
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService) => _medicoService = medicoService;

    [HttpGet]
    [Authorize(Roles = "Administrador, Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MedicoDto.ListResponse>>>> GetAll()
    {
        var result = await _medicoService.ObtenerTodosAsync();
        return HandleResponse(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<MedicoDto.DetailResponse>>> Create([FromBody] MedicoDto.Create request)
    {
        var result = await _medicoService.CrearAsync(request);
        return HandleResponse(result, "Médico registrado correctamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _medicoService.EliminarAsync(id);
        return HandleSuccess("Médico eliminado.");
    }
}
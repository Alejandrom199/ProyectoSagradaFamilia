using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers;

[Authorize(Roles = "Administrador")]
public class MedicoController : BaseController
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService) => _medicoService = medicoService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MedicoDto.ListResponse>>>> GetAll()
    {
        var result = await _medicoService.ObtenerTodosAsync();
        return HandleResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MedicoDto.DetailResponse>>> Create([FromBody] MedicoDto.Create request)
    {
        // 💡 Ajustado: La interfaz devuelve DetailResponse para creación
        var result = await _medicoService.CrearAsync(request);
        return HandleResponse(result, "Médico registrado correctamente.");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _medicoService.EliminarAsync(id);
        return HandleSuccess("Médico eliminado.");
    }
}
namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class SistemaController : BaseController
{
    private readonly IAuditoriaService _auditoriaService;
    private readonly ILogSistemaService _logService;

    public SistemaController(IAuditoriaService auditoriaService, ILogSistemaService logService)
    {
        _auditoriaService = auditoriaService;
        _logService = logService;
    }

    [HttpGet("auditoria/tabla/{nombreTabla}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditoriaDto.Response>>>> AuditoriaTabla(string nombreTabla, [FromQuery] string pk)
    {
        var response = await _auditoriaService.ObtenerPorTablaAsync(nombreTabla, pk);
        return HandleResponse(response);
    }

    [HttpGet("logs/errores")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LogSistemaDto.Response>>>> ErroresRecientes()
    {
        var response = await _logService.ObtenerErroresRecientesAsync();
        return HandleResponse(response);
    }
}
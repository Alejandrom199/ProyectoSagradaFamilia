namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

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

    [HttpGet("auditoria")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditoriaDto.Response>>>> AuditoriaReciente([FromQuery] int top = 100)
    {
        var response = await _auditoriaService.ObtenerRecientesAsync(top);
        return HandleResponse(response);
    }

    [HttpGet("auditoria/mia")]
    [Authorize(Roles = "Administrador, Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditoriaDto.Response>>>> MiActividad()
    {
        var response = await _auditoriaService.ObtenerPorUsuarioAsync(UsuarioId);
        return HandleResponse(response);
    }

    [HttpGet("auditoria/tabla/{nombreTabla}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditoriaDto.Response>>>> AuditoriaTabla(string nombreTabla, [FromQuery] string? pk = null)
    {
        var response = await _auditoriaService.ObtenerPorTablaAsync(nombreTabla, pk);
        return HandleResponse(response);
    }

    [HttpGet("logs")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LogSistemaDto.Response>>>> LogsRecientes([FromQuery] int top = 100)
    {
        var response = await _logService.ObtenerRecientesAsync(top);
        return HandleResponse(response);
    }

    [HttpGet("logs/errores")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LogSistemaDto.Response>>>> ErroresRecientes()
    {
        var response = await _logService.ObtenerErroresRecientesAsync();
        return HandleResponse(response);
    }
}

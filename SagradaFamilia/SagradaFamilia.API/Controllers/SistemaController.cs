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
    private readonly IAuditoriaService    _auditoriaService;
    private readonly ILogSistemaService   _logService;
    private readonly IDashboardAdminService _dashboardService;
    private readonly IVersionService      _versionService;

    public SistemaController(
        IAuditoriaService    auditoriaService,
        ILogSistemaService   logService,
        IDashboardAdminService dashboardService,
        IVersionService      versionService)
    {
        _auditoriaService  = auditoriaService;
        _logService        = logService;
        _dashboardService  = dashboardService;
        _versionService     = versionService;
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<SistemaDashboardDto>>> Dashboard()
    {
        var data = await _dashboardService.ObtenerAsync();
        return HandleResponse(data);
    }

    // Endpoint liviano: no requiere rol Administrador ni trae datos sensibles,
    // solo el número de versión de la app para mostrarlo en toda la UI.
    [HttpGet("version")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<VersionDto>>> Version()
    {
        var data = await _versionService.ObtenerAsync();
        return HandleResponse(data);
    }

    [HttpGet("auditoria/paginado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<PagedResponse<AuditoriaDto.Response>>> AuditoriaPaginado(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool asc = false)
    {
        var (items, total) = await _auditoriaService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
        return PagedResponse<AuditoriaDto.Response>.Ok(items, total, page, pageSize);
    }

    [HttpGet("logs/paginado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<PagedResponse<LogSistemaDto.Response>>> LogsPaginado(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool asc = false)
    {
        var (items, total) = await _logService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
        return PagedResponse<LogSistemaDto.Response>.Ok(items, total, page, pageSize);
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

    [HttpGet("auditoria/exportar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ExportarAuditoriaExcel()
    {
        var bytes = await _auditoriaService.ExportarExcelAsync();
        string filename = $"auditoria-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }

    [HttpGet("auditoria/mia/exportar")]
    [Authorize(Roles = "Administrador, Medico")]
    public async Task<IActionResult> ExportarMiActividadExcel()
    {
        var bytes = await _auditoriaService.ExportarExcelPorUsuarioAsync(UsuarioId);
        string filename = $"mi-actividad-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
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

    [HttpGet("logs/exportar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ExportarLogsExcel()
    {
        var bytes = await _logService.ExportarExcelAsync();
        string filename = $"eventos-sistema-{DateTime.Now:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
    }
}

namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Reporting.Excel.Documents;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class LogSistemaService : ILogSistemaService
{
    private readonly ILogSistemaRepository _logSistemaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<LogSistemaService> _logger;

    public LogSistemaService(
        ILogSistemaRepository logSistemaRepository,
        IMapper mapper,
        ILogger<LogSistemaService> logger)
    {
        _logSistemaRepository = logSistemaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<LogSistemaDto.Response>> ObtenerRecientesAsync(int top = 100)
    {
        var logs = await _logSistemaRepository.ObtenerRecientesAsync(top);
        return _mapper.Map<IEnumerable<LogSistemaDto.Response>>(logs);
    }

    public async Task<byte[]> ExportarExcelAsync()
    {
        var logs = await ObtenerRecientesAsync(500);
        return new LogsExportDocument(logs).GenerarBytes();
    }

    public async Task<(IEnumerable<LogSistemaDto.Response> Items, int TotalItems)> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _logSistemaRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending);
        return (_mapper.Map<IEnumerable<LogSistemaDto.Response>>(items), total);
    }

    public async Task<IEnumerable<LogSistemaDto.Response>> ObtenerErroresRecientesAsync(int top = 50)
    {
        var logs = await _logSistemaRepository.ObtenerErroresRecientesAsync(top);
        return _mapper.Map<IEnumerable<LogSistemaDto.Response>>(logs);
    }

    public async Task<IEnumerable<LogSistemaDto.Response>> ObtenerPorNivelAsync(string nivel)
    {
        var logs = await _logSistemaRepository.ObtenerPorNivelAsync(nivel);
        return _mapper.Map<IEnumerable<LogSistemaDto.Response>>(logs);
    }

    public async Task RegistrarEventoAsync(string nivel, string mensaje, string endpoint, int? usuarioId = null)
    {
        try
        {
            await _logSistemaRepository.RegistrarAsync(new LogSistema
            {
                FechaHora = DateTime.UtcNow,
                Nivel = nivel,
                Mensaje = mensaje,
                Endpoint = endpoint,
                UsuarioId = usuarioId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo registrar el evento de sistema: {Mensaje}", mensaje);
        }
    }
}

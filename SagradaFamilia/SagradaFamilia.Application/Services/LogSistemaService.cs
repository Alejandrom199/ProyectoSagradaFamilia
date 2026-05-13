namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
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

    public async Task<IEnumerable<LogSistemaDto.Response>> ObtenerErroresRecientesAsync(int top = 50)
    {
        _logger.LogInformation("Consultando los {Top} errores más recientes registrados en el sistema.", top);

        var logs = await _logSistemaRepository.ObtenerErroresRecientesAsync(top);

        _logger.LogInformation("Se recuperaron {Count} registros de errores críticos/excepciones.", logs.Count());

        return _mapper.Map<IEnumerable<LogSistemaDto.Response>>(logs);
    }

    public async Task<IEnumerable<LogSistemaDto.Response>> ObtenerPorNivelAsync(string nivel)
    {
        _logger.LogInformation("Iniciando filtrado de logs del sistema por nivel: {Nivel}", nivel);

        var logs = await _logSistemaRepository.ObtenerPorNivelAsync(nivel);

        _logger.LogInformation("Consulta finalizada. Registros encontrados para el nivel '{Nivel}': {Count}",
            nivel, logs.Count());

        return _mapper.Map<IEnumerable<LogSistemaDto.Response>>(logs);
    }
}
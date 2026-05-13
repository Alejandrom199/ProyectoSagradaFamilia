namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AuditoriaService> _logger;

    public AuditoriaService(
        IAuditoriaRepository auditoriaRepository,
        IMapper mapper,
        ILogger<AuditoriaService> logger)
    {
        _auditoriaRepository = auditoriaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AuditoriaDto.Response>> ObtenerPorTablaAsync(string nombreTabla, string clavePrimaria)
    {
        _logger.LogInformation("Consultando historial de auditoría - Tabla: {Tabla}, ID: {PK}", nombreTabla, clavePrimaria);

        var logs = await _auditoriaRepository.ObtenerPorTablaAsync(nombreTabla, clavePrimaria);
        return _mapper.Map<IEnumerable<AuditoriaDto.Response>>(logs);
    }

    public async Task<IEnumerable<AuditoriaDto.Response>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        _logger.LogInformation("Consultando actividad del Usuario ID: {Id}", usuarioId);

        var logs = await _auditoriaRepository.ObtenerPorUsuarioAsync(usuarioId);
        return _mapper.Map<IEnumerable<AuditoriaDto.Response>>(logs);
    }

    public async Task RegistrarAsync(AuditoriaDto.Create request)
    {
        _logger.LogInformation("Registrando auditoría: Acción '{Accion}' en Tabla '{Tabla}'", request.Accion, request.Tabla);

        try
        {
            var auditoria = _mapper.Map<Auditoria>(request);

            await _auditoriaRepository.RegistrarAsync(auditoria);

            _logger.LogDebug("Evento de auditoría para la tabla {Tabla} persistido correctamente.", request.Tabla);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo crítico al registrar auditoría en la tabla {Tabla}", request.Tabla);
            throw;
        }
    }
}
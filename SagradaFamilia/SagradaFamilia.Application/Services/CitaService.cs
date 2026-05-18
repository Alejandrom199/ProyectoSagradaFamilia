namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CitaService> _logger;

    public CitaService(
        ICitaRepository citaRepository,
        IMapper mapper,
        ILogger<CitaService> logger)
    {
        _citaRepository = citaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CitaDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando información de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        return _mapper.Map<CitaDto.Response>(cita);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorNinoIdAsync(int ninoId)
    {
        _logger.LogInformation("Consultando historial de citas para el niño ID: {NinoId}", ninoId);

        var citas = await _citaRepository.ObtenerPorNinoIdAsync(ninoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorMedicoIdAsync(int medicoId, DateOnly fecha)
    {
        _logger.LogInformation("Consultando agenda del médico ID: {MedicoId} para la fecha: {Fecha}", medicoId, fecha);

        var citas = await _citaRepository.ObtenerPorMedicoIdAsync(medicoId, fecha);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPendientesPorMedicoAsync(int medicoId)
    {
        _logger.LogInformation("Obteniendo todas las citas pendientes para el médico ID: {MedicoId}", medicoId);

        var citas = await _citaRepository.ObtenerPendientesPorMedicoAsync(medicoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<CitaDto.Response> CrearAsync(CitaDto.Create request)
    {
        _logger.LogInformation("Intentando agendar nueva cita para el Niño ID: {NinoId} con el Médico ID: {MedicoId}",
            request.NinoId, request.MedicoId);

        var cita = _mapper.Map<Cita>(request);
        cita.Estado = EstadoCita.Pendiente; // Toda cita nueva inicia como pendiente

        var creada = await _citaRepository.CrearAsync(cita);

        _logger.LogInformation("Cita agendada exitosamente con ID: {Id} para la fecha/hora: {FechaHora}",
            creada.Id, creada.FechaHora);

        // Recuperamos con navegación para devolver nombres de Niño/Médico en el DTO
        var citaCompleta = await _citaRepository.ObtenerPorIdAsync(creada.Id);
        return _mapper.Map<CitaDto.Response>(citaCompleta!);
    }

    public async Task<CitaDto.Response> ActualizarAsync(int id, CitaDto.Update request)
    {
        _logger.LogInformation("Iniciando actualización/reprogramación de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        // Mapeamos los cambios (Médico, FechaHora, Motivo)
        _mapper.Map(request, cita);

        var actualizada = await _citaRepository.ActualizarAsync(cita);

        _logger.LogInformation("Cita ID: {Id} actualizada correctamente. Nueva Fecha/Hora: {FechaHora}",
            id, actualizada.FechaHora);

        var citaCompleta = await _citaRepository.ObtenerPorIdAsync(actualizada.Id);
        return _mapper.Map<CitaDto.Response>(citaCompleta!);
    }

    public async Task ActualizarEstadoAsync(int id, EstadoCita nuevoEstado)
    {
        _logger.LogInformation("Cambiando estado de la cita ID: {Id} a {Estado}", id, nuevoEstado);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        var estadoAnterior = cita.Estado;
        cita.Estado = nuevoEstado;

        await _citaRepository.ActualizarAsync(cita);

        _logger.LogInformation("Estado de la cita ID: {Id} cambiado exitosamente de {Anterior} a {Nuevo}",
            id, estadoAnterior, nuevoEstado);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Se ha solicitado la eliminación de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        await _citaRepository.EliminarAsync(cita.Id);

        _logger.LogInformation("Cita ID: {Id} eliminada correctamente del sistema.", id);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorPadreIdAsync(int padreId)
    {
        _logger.LogInformation("Consultando citas de los hijos del padre ID: {PadreId}", padreId);

        var citas = await _citaRepository.ObtenerPorPadreIdAsync(padreId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerProximasPorMedicoAsync(int medicoId)
    {
        _logger.LogInformation("Consultando agenda futura del médico ID: {MedicoId}", medicoId);
        var citas = await _citaRepository.ObtenerPendientesPorMedicoAsync(medicoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }
}
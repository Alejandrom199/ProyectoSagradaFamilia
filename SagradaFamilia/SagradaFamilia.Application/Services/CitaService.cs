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
    private readonly IMedicoRepository _medicoRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CitaService> _logger;

    public CitaService(
        ICitaRepository citaRepository,
        IMedicoRepository medicoRepository,
        IParametroRepository parametroRepository,
        IMapper mapper,
        ILogger<CitaService> logger)
    {
        _citaRepository = citaRepository;
        _medicoRepository = medicoRepository;
        _parametroRepository = parametroRepository;
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

    public async Task<IEnumerable<CitaDto.Response>> ObtenerHistorialPorMedicoAsync(int usuarioId)
    {
        _logger.LogInformation("Consultando historial completo del médico ID: {UsuarioId}", usuarioId);
        var citas = await _citaRepository.ObtenerHistorialPorMedicoAsync(usuarioId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorNinoIdAsync(int ninoId)
    {
        _logger.LogInformation("Consultando historial de citas para el niño ID: {NinoId}", ninoId);

        var citas = await _citaRepository.ObtenerPorNinoIdAsync(ninoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<(IEnumerable<CitaDto.Response> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(
        int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _citaRepository.ObtenerPaginadoPorNinoAsync(ninoId, page, pageSize, search, sortBy, ascending);
        return (_mapper.Map<IEnumerable<CitaDto.Response>>(items), total);
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

    public async Task<CitaDto.Response> CrearAsync(CitaDto.Create request, int usuarioId)
    {
        _logger.LogInformation("Intentando agendar nueva cita para el Niño ID: {NinoId} con el Médico UsuarioId: {UsuarioId}",
            request.NinoId, usuarioId);

        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Médico", usuarioId);

        await ValidarHorarioAtencionAsync(request.FechaHora);

        if (request.FechaHoraFin <= request.FechaHora)
            throw new BusinessException("La hora de terminación debe ser posterior a la hora de inicio.");

        var haySolapamiento = await _citaRepository.ExisteTraslapeAsync(
            medico.Id, request.FechaHora, request.FechaHoraFin);

        if (haySolapamiento)
            throw new BusinessException(
                $"El médico ya tiene una cita programada que se solapa con el horario " +
                $"{request.FechaHora:HH:mm}–{request.FechaHoraFin:HH:mm}. " +
                $"Verifique la agenda antes de agendar.");

        var cita = _mapper.Map<Cita>(request);
        cita.MedicoId = medico.Id;
        cita.Estado = EstadoCita.Pendiente;
        cita.UsuarioCreacionId = usuarioId;

        var creada = await _citaRepository.CrearAsync(cita);

        _logger.LogInformation("Cita agendada exitosamente con ID: {Id} para la fecha/hora: {FechaHora}",
            creada.Id, creada.FechaHora);

        var citaCompleta = await _citaRepository.ObtenerPorIdAsync(creada.Id);
        return _mapper.Map<CitaDto.Response>(citaCompleta!);
    }

    public async Task<CitaDto.Response> ActualizarAsync(int id, CitaDto.Update request, int usuarioId)
    {
        _logger.LogInformation("Iniciando actualización/reprogramación de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        if (request.FechaHoraFin <= request.FechaHora)
            throw new BusinessException("La hora de terminación debe ser posterior a la hora de inicio.");

        var haySolapamiento = await _citaRepository.ExisteTraslapeAsync(
            cita.MedicoId, request.FechaHora, request.FechaHoraFin, excluirCitaId: id);

        if (haySolapamiento)
            throw new BusinessException(
                $"El médico ya tiene una cita que se solapa con el horario " +
                $"{request.FechaHora:HH:mm}–{request.FechaHoraFin:HH:mm}.");

        _mapper.Map(request, cita);

        var citaMap = _mapper.Map<Cita>(request);
        cita.UsuarioModificacionId = usuarioId;

        var actualizada = await _citaRepository.ActualizarAsync(citaMap);

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

        if (nuevoEstado != EstadoCita.Cancelada && DateTime.Now < cita.FechaHora)
            throw new BusinessException("No es posible gestionar esta cita antes de su fecha y hora programada.");

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

    private async Task ValidarHorarioAtencionAsync(DateTime fechaHora)
    {
        var pHoraInicio = await _parametroRepository.ObtenerPorGrupoYCodigoAsync("HORARIO_ATENCION", "HORA_INICIO");
        var pHoraFin = await _parametroRepository.ObtenerPorGrupoYCodigoAsync("HORARIO_ATENCION", "HORA_FIN");
        var pDiasHabiles = await _parametroRepository.ObtenerPorGrupoYCodigoAsync("HORARIO_ATENCION", "DIAS_HABILES");

        if (pHoraInicio is not null && TimeOnly.TryParse(pHoraInicio.Valor, out var horaInicio))
        {
            if (TimeOnly.FromDateTime(fechaHora) < horaInicio)
                throw new BusinessException($"Las citas no pueden agendarse antes de las {pHoraInicio.Valor} horas.");
        }

        if (pHoraFin is not null && TimeOnly.TryParse(pHoraFin.Valor, out var horaFin))
        {
            if (TimeOnly.FromDateTime(fechaHora) >= horaFin)
                throw new BusinessException($"Las citas no pueden agendarse después de las {pHoraFin.Valor} horas.");
        }

        if (pDiasHabiles is not null)
        {
            var diasHabiles = pDiasHabiles.Valor
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => int.TryParse(d.Trim(), out var n) ? (int?)n : null)
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .ToList();

            // DayOfWeek: Sunday=0, Monday=1 ... pero el parámetro usa 1=Lunes, 7=Domingo
            int diaSemana = fechaHora.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fechaHora.DayOfWeek;

            if (diasHabiles.Count > 0 && !diasHabiles.Contains(diaSemana))
                throw new BusinessException("Las citas solo pueden agendarse en días hábiles de atención.");
        }
    }

}
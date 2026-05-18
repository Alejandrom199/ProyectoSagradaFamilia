namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class PrescripcionService : IPrescripcionService
{
    private readonly IPrescripcionRepository _prescripcionRepository;
    private readonly INinoRepository _ninoRepository;
    private readonly ICitaRepository _citaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescripcionService> _logger;

    public PrescripcionService(
        IPrescripcionRepository prescripcionRepository,
        INinoRepository ninoRepository,
        ICitaRepository citaRepository,
        IMapper mapper,
        ILogger<PrescripcionService> logger)
    {
        _prescripcionRepository = prescripcionRepository;
        _ninoRepository = ninoRepository;
        _citaRepository = citaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PrescripcionDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Iniciando consulta de prescripción médica con ID: {Id}", id);

        var prescripcion = await _prescripcionRepository.ObtenerPorIdAsync(id);

        if (prescripcion == null)
        {
            _logger.LogError("No se encontró la prescripción médica con ID: {Id}", id);
            throw new NotFoundException("Prescripción", id);
        }

        return _mapper.Map<PrescripcionDto.Response>(prescripcion);
    }

    public async Task<IEnumerable<PrescripcionDto.Response>> ObtenerHistorialPorNinoAsync(int ninoId)
    {
        _logger.LogInformation("Consultando historial de recetas para el niño ID: {NinoId}", ninoId);

        // 💡 Validación de existencia usando ObtenerPorIdAsync (ya que ExisteAsync no existe)
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
        if (nino == null)
        {
            _logger.LogError("Fallo al obtener historial: El niño ID {NinoId} no existe en el sistema.", ninoId);
            throw new NotFoundException("Niño", ninoId);
        }

        // 💡 Usamos el nombre exacto del repositorio: ObtenerHistorialPorNinoAsync
        var prescripciones = await _prescripcionRepository.ObtenerHistorialPorNinoAsync(ninoId);
        return _mapper.Map<IEnumerable<PrescripcionDto.Response>>(prescripciones);
    }

    public async Task<IEnumerable<PrescripcionDto.Response>> ObtenerPorMedicoAsync(int medicoId)
    {
        _logger.LogInformation("Consultando todas las prescripciones emitidas por el médico ID: {MedicoId}", medicoId);

        // 💡 Usamos el nombre exacto: ObtenerPorMedicoAsync
        var prescripciones = await _prescripcionRepository.ObtenerPorMedicoAsync(medicoId);
        return _mapper.Map<IEnumerable<PrescripcionDto.Response>>(prescripciones);
    }

    public async Task<PrescripcionDto.Response> CrearAsync(PrescripcionDto.Create request, int medicoId)
    {
        _logger.LogInformation("Generando nueva prescripción para la cita ID: {CitaId}", request.CitaId);

        // 1. Validar que la cita exista y obtener contexto
        var cita = await _citaRepository.ObtenerPorIdAsync(request.CitaId)
            ?? throw new NotFoundException("Cita", request.CitaId);

        // 2. Validar que la cita esté en curso
        if (cita.Estado != EstadoCita.EnCurso)
        {
            _logger.LogWarning("Intento de prescribir en cita ID {Id} con estado {Estado}", cita.Id, cita.Estado);
            throw new BusinessException("Solo se puede prescribir en una cita que esté en curso.");
        }

        // 3. Mapear y asignar contexto desde la cita
        var prescripcion = _mapper.Map<Prescripcion>(request);
        prescripcion.NinoId = cita.NinoId;    // ← desde la cita
        prescripcion.MedicoId = medicoId;        // ← desde el token

        var creada = await _prescripcionRepository.CrearAsync(prescripcion);

        _logger.LogInformation("Prescripción ID: {Id} registrada para cita ID: {CitaId}", creada.Id, cita.Id);

        var prescripcionCompleta = await _prescripcionRepository.ObtenerPorIdAsync(creada.Id);
        return _mapper.Map<PrescripcionDto.Response>(prescripcionCompleta!);
    }

    public async Task<PrescripcionDto.Response> ActualizarAsync(int id, PrescripcionDto.Update request)
    {
        _logger.LogInformation("Iniciando actualización de la prescripción ID: {Id}", id);

        var prescripcion = await _prescripcionRepository.ObtenerPorIdAsync(id);
        if (prescripcion == null)
        {
            _logger.LogError("Fallo al actualizar: Prescripción ID {Id} no encontrada.", id);
            throw new NotFoundException("Prescripción", id);
        }

        // Aplicamos los cambios del DTO a la entidad
        _mapper.Map(request, prescripcion);

        var actualizada = await _prescripcionRepository.ActualizarAsync(prescripcion);

        _logger.LogInformation("Contenido de la prescripción ID: {Id} actualizado correctamente.", id);

        var prescripcionCompleta = await _prescripcionRepository.ObtenerPorIdAsync(actualizada.Id);
        return _mapper.Map<PrescripcionDto.Response>(prescripcionCompleta!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Iniciando proceso de eliminación de la prescripción ID: {Id}", id);

        var prescripcion = await _prescripcionRepository.ObtenerPorIdAsync(id);
        if (prescripcion == null)
        {
            _logger.LogError("No se puede eliminar: Prescripción ID {Id} no encontrada.", id);
            throw new NotFoundException("Prescripción", id);
        }

        await _prescripcionRepository.EliminarAsync(prescripcion.Id);

        _logger.LogInformation("Prescripción ID: {Id} eliminada lógicamente con éxito.", id);
    }
}
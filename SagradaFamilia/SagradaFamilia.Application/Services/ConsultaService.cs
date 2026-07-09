namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly ICitaRepository _citaRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ConsultaService> _logger;

    public ConsultaService(
        IConsultaRepository consultaRepository,
        ICitaRepository citaRepository,
        IMedicoRepository medicoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ConsultaService> logger)
    {
        _consultaRepository = consultaRepository;
        _citaRepository = citaRepository;
        _medicoRepository = medicoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ConsultaDto.Response> IniciarAsync(int citaId, int usuarioId)
    {
        _logger.LogInformation("Iniciando consulta para la cita ID: {CitaId}", citaId);

        var cita = await _citaRepository.ObtenerPorIdAsync(citaId)
            ?? throw new NotFoundException("Cita", citaId);

        if (cita.Estado != EstadoCita.Pendiente)
            throw new BusinessException("Solo se puede iniciar una consulta sobre una cita pendiente.");

        if (cita.Consulta != null)
            throw new BusinessException("Esta cita ya tiene una consulta iniciada.");

        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Médico", usuarioId);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var consulta = new Consulta
            {
                CitaId = cita.Id,
                NinoId = cita.NinoId,
                MedicoId = medico.Id,
                Estado = EstadoConsulta.EnCurso,
                Motivo = cita.Motivo,
                UsuarioCreacionId = usuarioId
            };
            var creada = await _consultaRepository.CrearAsync(consulta);

            cita.Estado = EstadoCita.EnCurso;
            cita.UsuarioModificacionId = usuarioId;
            await _citaRepository.ActualizarAsync(cita);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Consulta ID: {Id} iniciada para la cita ID: {CitaId}", creada.Id, citaId);

            var completa = await _consultaRepository.ObtenerPorIdAsync(creada.Id);
            return _mapper.Map<ConsultaDto.Response>(completa!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al iniciar la consulta para la cita ID: {CitaId}. Se realizó Rollback.", citaId);
            throw;
        }
    }

    public async Task<ConsultaDto.Response> ActualizarAsync(int id, ConsultaDto.Actualizar request, int usuarioId)
    {
        _logger.LogInformation("Actualizando datos clínicos de la consulta ID: {Id}", id);

        var consulta = await _consultaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Consulta", id);

        if (consulta.Estado != EstadoConsulta.EnCurso)
            throw new BusinessException("Solo se puede editar una consulta que esté en curso.");

        _mapper.Map(request, consulta);
        consulta.UsuarioModificacionId = usuarioId;

        var actualizada = await _consultaRepository.ActualizarAsync(consulta);

        var completa = await _consultaRepository.ObtenerPorIdAsync(actualizada.Id);
        return _mapper.Map<ConsultaDto.Response>(completa!);
    }

    public async Task<IEnumerable<ConsultaDto.Response>> ObtenerHistorialPorNinoAsync(int ninoId)
    {
        _logger.LogInformation("Consultando historial de consultas para el niño ID: {NinoId}", ninoId);

        var consultas = await _consultaRepository.ObtenerPorNinoIdAsync(ninoId);
        return _mapper.Map<IEnumerable<ConsultaDto.Response>>(consultas);
    }

    public async Task<ConsultaDto.Response> CompletarAsync(int id, int usuarioId)
    {
        _logger.LogInformation("Completando consulta ID: {Id}", id);

        var consulta = await _consultaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Consulta", id);

        if (consulta.Estado != EstadoConsulta.EnCurso)
            throw new BusinessException("Solo se puede completar una consulta que esté en curso.");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            consulta.Estado = EstadoConsulta.Completada;
            consulta.UsuarioModificacionId = usuarioId;
            await _consultaRepository.ActualizarAsync(consulta);

            var cita = consulta.Cita;
            cita.Estado = EstadoCita.Completada;
            cita.UsuarioModificacionId = usuarioId;
            await _citaRepository.ActualizarAsync(cita);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Consulta ID: {Id} completada. Cita ID: {CitaId} marcada como Completada.", id, cita.Id);

            var completa = await _consultaRepository.ObtenerPorIdAsync(id);
            return _mapper.Map<ConsultaDto.Response>(completa!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al completar la consulta ID: {Id}. Se realizó Rollback.", id);
            throw;
        }
    }
}

namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class NinoService : INinoService
{
    private readonly INinoRepository _ninoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<NinoService> _logger;

    public NinoService(
        INinoRepository ninoRepository,
        IMapper mapper,
        ILogger<NinoService> logger)
    {
        _ninoRepository = ninoRepository;
        _mapper = mapper;
        _logger = logger;
    }


    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando el listado global de niños en el sistema.");

        var ninos = await _ninoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }

    public async Task<NinoDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando información detallada del niño con ID: {Id}", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        return _mapper.Map<NinoDto.DetailResponse>(nino);
    }

    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorPadreIdAsync(int padreId)
    {
        _logger.LogInformation("Obteniendo lista de hijos para el Padre ID: {PadreId}", padreId);

        var ninos = await _ninoRepository.ObtenerPorPadreIdAsync(padreId);
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }

    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorMedicoIdAsync(int medicoId)
    {
        _logger.LogInformation("Obteniendo lista de pacientes para el Médico ID: {MedicoId}", medicoId);

        var ninos = await _ninoRepository.ObtenerPorMedicoIdAsync(medicoId);
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }


    public async Task<bool> PerteneceAPadreAsync(int ninoId, int padreId)
    {
        _logger.LogDebug("Verificando si el niño {NinoId} pertenece al padre {PadreId}", ninoId, padreId);
        return await _ninoRepository.PerteneceAPadreAsync(ninoId, padreId);
    }

    public async Task<bool> PerteneceAMedicoAsync(int ninoId, int medicoId)
    {
        _logger.LogDebug("Verificando si el niño {NinoId} es paciente del médico {MedicoId}", ninoId, medicoId);
        return await _ninoRepository.PerteneceAMedicoAsync(ninoId, medicoId);
    }


    public async Task<NinoDto.DetailResponse> CrearAsync(NinoDto.Create request)
    {
        _logger.LogInformation("Registrando nuevo niño: {Nombre} {Apellido}", request.Nombre, request.Apellido);

        var nino = _mapper.Map<Nino>(request);

        var creado = await _ninoRepository.CrearAsync(nino);

        _logger.LogInformation("Niño registrado exitosamente con ID: {Id} y asignado al Médico ID: {MedicoId}",
            creado.Id, request.MedicoId);

        var ninoCompleto = await _ninoRepository.ObtenerPorIdAsync(creado.Id);
        return _mapper.Map<NinoDto.DetailResponse>(ninoCompleto!);
    }

    public async Task<NinoDto.DetailResponse> ActualizarAsync(int id, NinoDto.Update request)
    {
        _logger.LogInformation("Iniciando actualización de datos para el niño ID: {Id}", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        _mapper.Map(request, nino);

        var actualizado = await _ninoRepository.ActualizarAsync(nino);

        _logger.LogInformation("Datos del niño ID: {Id} actualizados correctamente.", id);

        var ninoCompleto = await _ninoRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<NinoDto.DetailResponse>(ninoCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Se ha solicitado la eliminación lógica del niño ID: {Id}", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        await _ninoRepository.EliminarAsync(nino.Id);

        _logger.LogInformation("Niño ID: {Id} eliminado lógicamente del sistema.", id);
    }
}
namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class ParametroService : IParametroService
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ParametroService> _logger;

    public ParametroService(
        IParametroRepository parametroRepository,
        IMapper mapper,
        ILogger<ParametroService> logger)
    {
        _parametroRepository = parametroRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ParametroDto.Response>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando todos los parámetros del sistema.");
        var parametros = await _parametroRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<ParametroDto.Response>>(parametros);
    }

    public async Task<IEnumerable<ParametroDto.Response>> ObtenerPorGrupoAsync(string grupo)
    {
        _logger.LogInformation("Consultando parámetros del grupo: {Grupo}", grupo);
        var parametros = await _parametroRepository.ObtenerPorGrupoAsync(grupo);
        return _mapper.Map<IEnumerable<ParametroDto.Response>>(parametros);
    }

    public async Task<ParametroDto.Response?> ObtenerPorGrupoYCodigoAsync(string grupo, string codigo)
    {
        _logger.LogInformation("Consultando parámetro {Grupo}/{Codigo}", grupo, codigo);
        var parametro = await _parametroRepository.ObtenerPorGrupoYCodigoAsync(grupo, codigo);
        return parametro is null ? null : _mapper.Map<ParametroDto.Response>(parametro);
    }

    public async Task<ParametroDto.Response> ObtenerPorIdAsync(int id)
    {
        var parametro = await _parametroRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Parámetro", id);
        return _mapper.Map<ParametroDto.Response>(parametro);
    }

    public async Task<ParametroDto.Response> CrearAsync(ParametroDto.Create request)
    {
        _logger.LogInformation("Creando parámetro {Grupo}/{Codigo}", request.Grupo, request.Codigo);

        var existente = await _parametroRepository.ObtenerPorGrupoYCodigoAsync(request.Grupo, request.Codigo);
        if (existente is not null)
            throw new BusinessException($"Ya existe un parámetro con el código '{request.Codigo}' en el grupo '{request.Grupo}'.");

        var parametro = _mapper.Map<ParametroSistema>(request);
        var creado = await _parametroRepository.CrearAsync(parametro);
        return _mapper.Map<ParametroDto.Response>(creado);
    }

    public async Task<ParametroDto.Response> ActualizarAsync(int id, ParametroDto.Update request)
    {
        _logger.LogInformation("Actualizando parámetro ID: {Id}", id);

        var parametro = await _parametroRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Parámetro", id);

        _mapper.Map(request, parametro);
        var actualizado = await _parametroRepository.ActualizarAsync(parametro);
        return _mapper.Map<ParametroDto.Response>(actualizado);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Eliminando parámetro ID: {Id}", id);

        var parametro = await _parametroRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Parámetro", id);

        await _parametroRepository.EliminarAsync(parametro.Id);
    }
}

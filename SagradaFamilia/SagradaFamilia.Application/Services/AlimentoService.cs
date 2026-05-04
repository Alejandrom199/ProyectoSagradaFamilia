namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Alimentos;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class AlimentoService : IAlimentoService
{
    private readonly IAlimentoRepository _alimentoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AlimentoService> _logger;

    public AlimentoService(
        IAlimentoRepository alimentoRepository,
        IMapper mapper,
        ILogger<AlimentoService> logger)
    {
        _alimentoRepository = alimentoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AlimentoResponse>> ObtenerPorEdadAsync(int edadMeses)
    {
        var alimentos = await _alimentoRepository.ObtenerPorEdadAsync(edadMeses);
        return _mapper.Map<IEnumerable<AlimentoResponse>>(alimentos);
    }

    public async Task<IEnumerable<AlimentoResponse>> ObtenerTodosAsync()
    {
        var alimentos = await _alimentoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<AlimentoResponse>>(alimentos);
    }

    public async Task<AlimentoResponse> CrearAsync(CrearAlimentoRequest request)
    {
        var alimento = _mapper.Map<Alimento>(request);
        var creado = await _alimentoRepository.CrearAsync(alimento);
        _logger.LogInformation("Alimento creado con ID: {Id}", creado.Id);
        return _mapper.Map<AlimentoResponse>(creado);
    }

    public async Task<AlimentoResponse> ActualizarAsync(int id, CrearAlimentoRequest request)
    {
        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        alimento.Nombre = request.Nombre;
        alimento.CategoriaId = request.CategoriaId;
        alimento.Descripcion = request.Descripcion;
        alimento.EdadMinimaIntro = request.EdadMinimaIntro;
        alimento.EdadMaxima = request.EdadMaxima;
        alimento.Recomendacion = request.Recomendacion;

        var actualizado = await _alimentoRepository.ActualizarAsync(alimento);
        _logger.LogInformation("Alimento actualizado con ID: {Id}", id);
        return _mapper.Map<AlimentoResponse>(actualizado);
    }

    public async Task EliminarAsync(int id)
    {
        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        await _alimentoRepository.EliminarAsync(alimento.Id);
        _logger.LogInformation("Alimento eliminado con ID: {Id}", id);
    }

    public async Task<IEnumerable<CategoriaResponse>> ObtenerCategoriasAsync()
    {
        var categorias = await _alimentoRepository.ObtenerCategoriasAsync();
        return _mapper.Map<IEnumerable<CategoriaResponse>>(categorias);
    }

    public async Task<CategoriaResponse> CrearCategoriaAsync(CrearCategoriaRequest request)
    {
        var categoria = _mapper.Map<CategoriaAlimento>(request);
        var creada = await _alimentoRepository.CrearCategoriaAsync(categoria);
        _logger.LogInformation("Categoría creada con ID: {Id}", creada.Id);
        return _mapper.Map<CategoriaResponse>(creada);
    }
}
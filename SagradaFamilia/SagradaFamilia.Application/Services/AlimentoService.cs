namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
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

    public async Task<IEnumerable<AlimentoDto.Response>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo el catálogo completo de alimentos.");

        var alimentos = await _alimentoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
    }

    public async Task<AlimentoDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando alimento con ID: {Id}", id);

        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        return _mapper.Map<AlimentoDto.Response>(alimento);
    }

    public async Task<IEnumerable<AlimentoDto.Response>> ObtenerPorRangoEdadAsync(int edadMeses)
    {
        _logger.LogInformation("Consultando alimentos recomendados para la edad: {Edad} meses", edadMeses);

        var alimentos = await _alimentoRepository.ObtenerPorRangoEdadAsync(edadMeses); // Asumiendo que el repo mantiene este nombre
        return _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
    }

    public async Task<IEnumerable<AlimentoDto.Response>> ObtenerPorCategoriaAsync(int categoriaId)
    {
        _logger.LogInformation("Obteniendo alimentos pertenecientes a la categoría ID: {CategoriaId}", categoriaId);

        var alimentos = await _alimentoRepository.ObtenerPorCategoriaAsync(categoriaId); // Asumiendo que existe en el repo
        return _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
    }

    public async Task<AlimentoDto.Response> CrearAsync(AlimentoDto.Create request)
    {
        _logger.LogInformation("Iniciando creación de nuevo alimento: {Nombre}", request.Nombre);

        var alimento = _mapper.Map<Alimento>(request);
        alimento.Activo = true;

        var creado = await _alimentoRepository.CrearAsync(alimento);

        _logger.LogInformation("Alimento '{Nombre}' creado exitosamente con ID: {Id}", creado.Nombre, creado.Id);

        var alimentoCompleto = await _alimentoRepository.ObtenerPorIdAsync(creado.Id);
        return _mapper.Map<AlimentoDto.Response>(alimentoCompleto!);
    }

    public async Task<AlimentoDto.Response> ActualizarAsync(int id, AlimentoDto.Update request)
    {
        _logger.LogInformation("Iniciando actualización del alimento ID: {Id}", id);

        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        _mapper.Map(request, alimento);

        var actualizado = await _alimentoRepository.ActualizarAsync(alimento);

        _logger.LogInformation("Alimento ID: {Id} actualizado correctamente.", id);

        var alimentoCompleto = await _alimentoRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<AlimentoDto.Response>(alimentoCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Intentando eliminar lógicamente el alimento ID: {Id}", id);

        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        await _alimentoRepository.EliminarAsync(alimento.Id);

        _logger.LogInformation("Alimento ID: {Id} ha sido eliminado lógicamente del sistema.", id);
    }

    public async Task<IEnumerable<CategoriaDto.Response>> ObtenerCategoriasAsync()
    {
        _logger.LogInformation("Obteniendo lista de todas las categorías de alimentos.");

        var categorias = await _alimentoRepository.ObtenerCategoriasAsync();
        return _mapper.Map<IEnumerable<CategoriaDto.Response>>(categorias);
    }

    public async Task<CategoriaDto.Response> ObtenerCategoriaPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando categoría de alimento con ID: {Id}", id);

        var categoria = await _alimentoRepository.ObtenerCategoriaPorIdAsync(id)
            ?? throw new NotFoundException("Categoría de Alimento", id);

        return _mapper.Map<CategoriaDto.Response>(categoria);
    }

    public async Task<CategoriaDto.Response> CrearCategoriaAsync(CategoriaDto.Create request)
    {
        _logger.LogInformation("Creando nueva categoría: {Nombre}", request.Nombre);

        var categoria = _mapper.Map<CategoriaAlimento>(request);
        var creada = await _alimentoRepository.CrearCategoriaAsync(categoria);

        _logger.LogInformation("Categoría '{Nombre}' creada con ID: {Id}", creada.Nombre, creada.Id);

        return _mapper.Map<CategoriaDto.Response>(creada);
    }

    public async Task<CategoriaDto.Response> ActualizarCategoriaAsync(int id, CategoriaDto.Update request)
    {
        _logger.LogInformation("Actualizando categoría ID: {Id}", id);

        var categoria = await _alimentoRepository.ObtenerCategoriaPorIdAsync(id)
            ?? throw new NotFoundException("Categoría de Alimento", id);

        _mapper.Map(request, categoria);
        var actualizada = await _alimentoRepository.ActualizarCategoriaAsync(categoria);

        _logger.LogInformation("Categoría ID: {Id} actualizada con éxito.", id);

        return _mapper.Map<CategoriaDto.Response>(actualizada);
    }

    public async Task EliminarCategoriaAsync(int id)
    {
        _logger.LogWarning("Intentando eliminar categoría ID: {Id}", id);

        var categoria = await _alimentoRepository.ObtenerCategoriaPorIdAsync(id)
            ?? throw new NotFoundException("Categoría de Alimento", id);

        await _alimentoRepository.EliminarCategoriaAsync(categoria.Id);

        _logger.LogInformation("Categoría ID: {Id} eliminada exitosamente.", id);
    }
}
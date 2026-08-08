namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class CatalogoValorService : ICatalogoValorService
{
    private readonly ICatalogoValorRepository _catalogoValorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CatalogoValorService> _logger;

    public CatalogoValorService(
        ICatalogoValorRepository catalogoValorRepository,
        IMapper mapper,
        ILogger<CatalogoValorService> logger)
    {
        _catalogoValorRepository = catalogoValorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CatalogoValorDto.Response>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando todos los valores de catálogo.");
        var valores = await _catalogoValorRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<CatalogoValorDto.Response>>(valores);
    }

    public async Task<IEnumerable<CatalogoValorDto.Response>> ObtenerPorTipoAsync(string tipo)
    {
        _logger.LogInformation("Consultando valores de catálogo del tipo: {Tipo}", tipo);
        var valores = await _catalogoValorRepository.ObtenerPorTipoAsync(tipo);
        return _mapper.Map<IEnumerable<CatalogoValorDto.Response>>(valores);
    }

    public async Task<CatalogoValorDto.Response?> ObtenerPorTipoYCodigoAsync(string tipo, string codigo)
    {
        _logger.LogInformation("Consultando valor de catálogo {Tipo}/{Codigo}", tipo, codigo);
        var valor = await _catalogoValorRepository.ObtenerPorTipoYCodigoAsync(tipo, codigo);
        return valor is null ? null : _mapper.Map<CatalogoValorDto.Response>(valor);
    }

    public async Task<CatalogoValorDto.Response> ObtenerPorIdAsync(int id)
    {
        var valor = await _catalogoValorRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Valor de catálogo", id);
        return _mapper.Map<CatalogoValorDto.Response>(valor);
    }

    public async Task<CatalogoValorDto.Response> CrearAsync(CatalogoValorDto.Create request)
    {
        _logger.LogInformation("Creando valor de catálogo {Tipo}/{Codigo}", request.Tipo, request.Codigo);

        var existente = await _catalogoValorRepository.ObtenerPorTipoYCodigoAsync(request.Tipo, request.Codigo);
        if (existente is not null)
            throw new BusinessException($"Ya existe un valor con el código '{request.Codigo}' en el tipo '{request.Tipo}'.");

        var catalogoValor = _mapper.Map<CatalogoValor>(request);
        var creado = await _catalogoValorRepository.CrearAsync(catalogoValor);
        return _mapper.Map<CatalogoValorDto.Response>(creado);
    }

    public async Task<CatalogoValorDto.Response> ActualizarAsync(int id, CatalogoValorDto.Update request)
    {
        _logger.LogInformation("Actualizando valor de catálogo ID: {Id}", id);

        var catalogoValor = await _catalogoValorRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Valor de catálogo", id);

        _mapper.Map(request, catalogoValor);
        var actualizado = await _catalogoValorRepository.ActualizarAsync(catalogoValor);
        return _mapper.Map<CatalogoValorDto.Response>(actualizado);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Eliminando valor de catálogo ID: {Id}", id);

        var catalogoValor = await _catalogoValorRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Valor de catálogo", id);

        await _catalogoValorRepository.EliminarAsync(catalogoValor.Id);
    }
}

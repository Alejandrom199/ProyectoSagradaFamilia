namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class PlantillaCorreoService : IPlantillaCorreoService
{
    private readonly IPlantillaCorreoRepository _repository;
    private readonly IEventoCorreoRepository    _eventoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PlantillaCorreoService> _logger;

    public PlantillaCorreoService(
        IPlantillaCorreoRepository repository,
        IEventoCorreoRepository eventoRepository,
        IMapper mapper,
        ILogger<PlantillaCorreoService> logger)
    {
        _repository       = repository;
        _eventoRepository = eventoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PlantillaCorreoDto.Response>> ObtenerTodosAsync()
    {
        var plantillas = await _repository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<PlantillaCorreoDto.Response>>(plantillas);
    }

    public async Task<PlantillaCorreoDto.Response> ObtenerPorIdAsync(int id)
    {
        var plantilla = await _repository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Plantilla de correo", id);
        return _mapper.Map<PlantillaCorreoDto.Response>(plantilla);
    }

    public async Task<PlantillaCorreoDto.Response> CrearAsync(PlantillaCorreoDto.Create request)
    {
        _logger.LogInformation("Creando nueva plantilla de correo: {Nombre}", request.Nombre);

        // Solo valida unicidad de código cuando se especifica uno (plantillas del sistema)
        if (!string.IsNullOrWhiteSpace(request.Codigo))
        {
            var existente = await _repository.ObtenerPorCodigoAsync(request.Codigo);
            if (existente is not null)
                throw new BusinessException($"Ya existe una plantilla con el código '{request.Codigo}'.");
        }

        var plantilla = _mapper.Map<PlantillaCorreo>(request);
        var creada = await _repository.CrearAsync(plantilla);
        return _mapper.Map<PlantillaCorreoDto.Response>(creada);
    }

    public async Task<PlantillaCorreoDto.Response> ActualizarAsync(int id, PlantillaCorreoDto.Update request)
    {
        _logger.LogInformation("Actualizando plantilla de correo ID: {Id}", id);

        var plantilla = await _repository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Plantilla de correo", id);

        _mapper.Map(request, plantilla);
        var actualizada = await _repository.ActualizarAsync(plantilla);
        return _mapper.Map<PlantillaCorreoDto.Response>(actualizada);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Eliminando plantilla de correo ID: {Id}", id);

        var plantilla = await _repository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Plantilla de correo", id);

        var enUso = await _eventoRepository.EstaEnUsoAsync(id);
        if (enUso)
            throw new BusinessException(
                "No se puede eliminar esta plantilla porque está asignada a un evento de correo activo. Reasigná el evento a otra plantilla primero.");

        await _repository.EliminarAsync(plantilla.Id);
    }
}

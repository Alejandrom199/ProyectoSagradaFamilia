namespace SagradaFamilia.Application.Services;

using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;
using System.Text.Json;

public class EventoCorreoService : IEventoCorreoService
{
    private readonly IEventoCorreoRepository   _eventoRepository;
    private readonly IPlantillaCorreoRepository _plantillaRepository;
    private readonly ILogger<EventoCorreoService> _logger;

    public EventoCorreoService(
        IEventoCorreoRepository eventoRepository,
        IPlantillaCorreoRepository plantillaRepository,
        ILogger<EventoCorreoService> logger)
    {
        _eventoRepository    = eventoRepository;
        _plantillaRepository = plantillaRepository;
        _logger              = logger;
    }

    public async Task<IEnumerable<EventoCorreoDto.Response>> ObtenerTodosAsync()
    {
        var eventos = await _eventoRepository.ObtenerTodosConPlantillaAsync();
        return eventos.Select(MapearDto);
    }

    public async Task<EventoCorreoDto.Response> AsignarPlantillaAsync(int eventoId, int? plantillaId)
    {
        // Verificar que el evento existe
        var eventos = await _eventoRepository.ObtenerTodosConPlantillaAsync();
        var evento  = eventos.FirstOrDefault(e => e.Id == eventoId)
            ?? throw new NotFoundException("Evento de correo", eventoId);

        // Si se asigna una plantilla, verificar que existe y está activa
        if (plantillaId.HasValue)
        {
            var plantilla = await _plantillaRepository.ObtenerPorIdAsync(plantillaId.Value)
                ?? throw new NotFoundException("Plantilla de correo", plantillaId.Value);

            if (!plantilla.Activo)
                throw new BusinessException("No se puede asignar una plantilla inactiva a un evento.");
        }

        await _eventoRepository.ActualizarPlantillaAsync(eventoId, plantillaId);
        _logger.LogInformation(
            "Evento '{Codigo}' reasignado a plantilla ID: {PlantillaId}",
            evento.Codigo, plantillaId?.ToString() ?? "ninguna");

        // Recargar para devolver estado actualizado
        var eventosActualizados = await _eventoRepository.ObtenerTodosConPlantillaAsync();
        return MapearDto(eventosActualizados.First(e => e.Id == eventoId));
    }

    private static EventoCorreoDto.Response MapearDto(Domain.Entities.EventoCorreo e) => new()
    {
        Id                = e.Id,
        Codigo            = e.Codigo,
        Nombre            = e.Nombre,
        Descripcion       = e.Descripcion,
        Variables         = JsonSerializer.Deserialize<string[]>(e.Variables) ?? [],
        PlantillaCorreoId = e.PlantillaCorreoId,
        PlantillaNombre   = e.Plantilla?.Nombre,
        PlantillaActiva   = e.Plantilla?.Activo ?? false,
    };
}

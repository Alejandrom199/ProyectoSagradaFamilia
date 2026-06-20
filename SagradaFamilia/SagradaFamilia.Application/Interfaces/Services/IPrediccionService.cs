using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services;

public interface IPrediccionService
{
    Task<PrediccionDto.Response>        ObtenerPrediccionesAsync(int ninoId);
    Task<PrediccionDto.Health>          ObtenerEstadoServicioPrediccionAsync();
    Task<PrediccionDto.CurvasOms>       ObtenerCurvasOmsAsync(int ninoId);
}
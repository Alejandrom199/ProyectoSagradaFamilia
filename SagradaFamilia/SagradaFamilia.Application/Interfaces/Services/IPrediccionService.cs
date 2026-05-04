using SagradaFamilia.Application.DTOs.Predicciones;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IPrediccionService
    {
        Task<PrediccionResponse> ObtenerPrediccionesAsync(int ninoId);
        Task<PrediccionHealth> ObtenerEstadoServicioPrediccionAsync();
    }
}

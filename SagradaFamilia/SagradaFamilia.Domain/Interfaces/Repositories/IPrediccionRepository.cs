using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IPrediccionRepository
    {
        Task<IEnumerable<Prediccion>> ObtenerPorNinoAsync(int ninoId);
        Task<Prediccion?> ObtenerPorNinoYFechaAsync(int ninoId, DateOnly fechaObjetivo);
        Task GuardarPrediccionesAsync(IEnumerable<Prediccion> predicciones);
        Task ActualizarPesoRealAsync(int ninoId, DateOnly fechaMedicion, decimal pesoReal);
    }
}

using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IPrediccionRepository
    {
        Task<IEnumerable<Prediccion>> ObtenerPorNinoAsync(int ninoId);
        Task<Prediccion?> ObtenerPorNinoYFechaAsync(int ninoId, DateOnly fechaObjetivo, TipoReferencia tipo);
        Task GuardarPrediccionesAsync(IEnumerable<Prediccion> predicciones);
        Task ActualizarValorRealAsync(int ninoId, DateOnly fechaMedicion, decimal valorReal, TipoReferencia tipo);
    }
}

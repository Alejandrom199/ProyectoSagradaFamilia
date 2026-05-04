using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface INinoRepository
    {
        Task<Nino?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Nino>> ObtenerTodosAsync();
        Task<IEnumerable<Nino>> ObtenerPorRepresentanteAsync(int representanteId);
        Task<bool> PerteneceARepresentanteAsync(int ninoId, int representanteId);
        Task<Nino> CrearAsync(Nino nino);
        Task<Nino> ActualizarAsync(Nino nino);
        Task EliminarAsync(int id);
    }
}

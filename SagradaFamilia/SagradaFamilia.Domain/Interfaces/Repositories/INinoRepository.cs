using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface INinoRepository
    {
        Task<Nino?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Nino>> ObtenerTodosAsync();

        Task<IEnumerable<Nino>> ObtenerPorMedicoIdAsync(int medicoId);
        Task<IEnumerable<Nino>> ObtenerPorPadreIdAsync(int padreId);
        Task<bool> PerteneceAPadreAsync(int ninoId, int padreId);
        Task<bool> PerteneceAMedicoAsync(int ninoId, int medicoId);

        Task<Nino> CrearAsync(Nino nino);
        Task<Nino> ActualizarAsync(Nino nino);
        Task EliminarAsync(int id);
    }
}
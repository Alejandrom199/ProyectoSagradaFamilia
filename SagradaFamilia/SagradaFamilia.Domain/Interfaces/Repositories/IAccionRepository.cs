using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IAccionRepository
    {
        Task<Accion?> ObtenerPorIdAsync(int id);
        Task<Accion?> ObtenerPorNombreAsync(string nombre);
        Task<IEnumerable<Accion>> ObtenerTodosAsync();
    }
}

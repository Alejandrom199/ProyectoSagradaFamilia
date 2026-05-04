using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IRolRepository
    {
        Task<Rol?> ObtenerPorIdAsync(int id);
        Task<Rol?> ObtenerPorNombreAsync(string nombre);
        Task<IEnumerable<Rol>> ObtenerTodosAsync();
    }
}

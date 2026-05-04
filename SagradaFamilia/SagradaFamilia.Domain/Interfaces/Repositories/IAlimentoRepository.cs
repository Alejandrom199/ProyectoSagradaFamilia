using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IAlimentoRepository
    {
        Task<IEnumerable<Alimento>> ObtenerPorEdadAsync(int edadMeses);
        Task<IEnumerable<Alimento>> ObtenerTodosAsync();
        Task<Alimento?> ObtenerPorIdAsync(int id);
        Task<Alimento> CrearAsync(Alimento alimento);
        Task<Alimento> ActualizarAsync(Alimento alimento);
        Task EliminarAsync(int id);
        Task<IEnumerable<CategoriaAlimento>> ObtenerCategoriasAsync();
        Task<CategoriaAlimento?> ObtenerCategoriaPorIdAsync(int id);
        Task<CategoriaAlimento> CrearCategoriaAsync(CategoriaAlimento categoria);
        Task<CategoriaAlimento> ActualizarCategoriaAsync(CategoriaAlimento categoria);
        Task EliminarCategoriaAsync(int id);
    }
}

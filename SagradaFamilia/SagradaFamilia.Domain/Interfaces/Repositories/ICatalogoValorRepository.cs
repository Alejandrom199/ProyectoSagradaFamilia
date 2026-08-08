using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface ICatalogoValorRepository
    {
        Task<IEnumerable<CatalogoValor>> ObtenerTodosAsync();
        Task<IEnumerable<CatalogoValor>> ObtenerPorTipoAsync(string tipo);
        Task<CatalogoValor?> ObtenerPorTipoYCodigoAsync(string tipo, string codigo);
        Task<CatalogoValor?> ObtenerPorIdAsync(int id);
        Task<CatalogoValor> CrearAsync(CatalogoValor catalogoValor);
        Task<CatalogoValor> ActualizarAsync(CatalogoValor catalogoValor);
        Task EliminarAsync(int id);
    }
}

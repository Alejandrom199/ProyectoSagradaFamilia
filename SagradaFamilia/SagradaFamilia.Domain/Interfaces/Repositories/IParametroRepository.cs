using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IParametroRepository
    {
        Task<IEnumerable<ParametroSistema>> ObtenerTodosAsync();
        Task<IEnumerable<ParametroSistema>> ObtenerPorGrupoAsync(string grupo);
        Task<ParametroSistema?> ObtenerPorGrupoYCodigoAsync(string grupo, string codigo);
        Task<ParametroSistema?> ObtenerPorIdAsync(int id);
        Task<ParametroSistema> CrearAsync(ParametroSistema parametro);
        Task<ParametroSistema> ActualizarAsync(ParametroSistema parametro);
        Task EliminarAsync(int id);
    }
}

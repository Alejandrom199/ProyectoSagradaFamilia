using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IConsultaRepository
    {
        Task<Consulta?> ObtenerPorIdAsync(int id);
        Task<Consulta?> ObtenerPorCitaIdAsync(int citaId);
        Task<IEnumerable<Consulta>> ObtenerPorNinoIdAsync(int ninoId);
        Task<Consulta> CrearAsync(Consulta consulta);
        Task<Consulta> ActualizarAsync(Consulta consulta);
    }
}

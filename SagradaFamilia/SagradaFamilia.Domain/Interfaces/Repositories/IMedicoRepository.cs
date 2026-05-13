using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IMedicoRepository
    {
        Task<Medico?> ObtenerPorIdAsync(int id);
        Task<Medico?> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Medico>> ObtenerTodosAsync();
        Task<Medico?> ObtenerConPacientesAsync(int id);
        Task<Medico> CrearAsync(Medico medico);
        Task<Medico> ActualizarAsync(Medico medico);
        Task EliminarAsync(int id);
    }
}
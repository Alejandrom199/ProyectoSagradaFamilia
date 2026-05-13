using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface ILogSistemaRepository
    {
        Task<IEnumerable<LogSistema>> ObtenerErroresRecientesAsync(int top);
        Task<IEnumerable<LogSistema>> ObtenerPorNivelAsync(string nivel);

        Task RegistrarAsync(LogSistema log);
    }
}
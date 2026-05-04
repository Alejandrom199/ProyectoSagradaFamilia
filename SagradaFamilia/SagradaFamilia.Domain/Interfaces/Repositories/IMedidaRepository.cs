using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IMedidaRepository
    {
        Task<Medida?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Medida>> ObtenerPorNinoAsync(int ninoId);
        Task<bool> ExisteMedidaEnMesAsync(int ninoId, DateOnly fecha);
        Task<Medida> CrearAsync(Medida medida);
        Task<Medida> ActualizarAsync(Medida medida);
        Task EliminarAsync(int id);
    }
}

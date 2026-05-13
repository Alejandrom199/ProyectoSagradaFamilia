using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IMedidaRepository
    {
        Task<Medida?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Medida>> ObtenerPorNinoAsync(int ninoId);
        Task<Medida?> ObtenerUltimaMedidaAsync(int ninoId);
        Task<bool> ExisteMedidaEnMesAsync(int ninoId, int mes, int anio);

        Task<Medida> CrearAsync(Medida medida);
        Task<Medida> ActualizarAsync(Medida medida);
        Task EliminarAsync(int id);
    }
}

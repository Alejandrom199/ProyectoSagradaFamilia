using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IReporteGeneradoRepository
    {
        Task<int> ContarPorTipoYAnioAsync(TipoReporte tipo, int anio);
        Task RegistrarAsync(ReporteGenerado reporte);
    }
}

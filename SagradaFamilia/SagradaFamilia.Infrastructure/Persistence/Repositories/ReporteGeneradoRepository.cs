using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class ReporteGeneradoRepository : IReporteGeneradoRepository
    {
        private readonly AppDbContext _context;

        public ReporteGeneradoRepository(AppDbContext context) => _context = context;

        public async Task<int> ContarPorTipoYAnioAsync(TipoReporte tipo, int anio) =>
            await _context.ReportesGenerados
                .CountAsync(r => r.Tipo == tipo && r.FechaGeneracion.Year == anio);

        public async Task RegistrarAsync(ReporteGenerado reporte)
        {
            _context.ReportesGenerados.Add(reporte);
            await _context.SaveChangesAsync();
        }
    }
}

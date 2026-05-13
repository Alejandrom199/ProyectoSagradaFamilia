using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class OmsRepository : IOmsRepository
    {
        private readonly AppDbContext _context;

        public OmsRepository(AppDbContext context) => _context = context;

        public async Task<OmsReferencia?> ObtenerReferenciaAsync(char sexo, int edadMeses, TipoReferencia tipo) =>
            await _context.OmsReferencias
                .Where(o => o.Sexo == sexo
                         && o.EdadMeses >= edadMeses
                         && o.Tipo == tipo)
                .OrderBy(o => o.EdadMeses) 
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<OmsReferencia>> ObtenerCurvaCompletaAsync(char sexo, TipoReferencia tipo) =>
            await _context.OmsReferencias
                .Where(o => o.Sexo == sexo && o.Tipo == tipo)
                .OrderBy(o => o.EdadMeses) 
                .ToListAsync();
    }
}
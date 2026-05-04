using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class OmsRepository : IOmsRepository
    {
        private readonly AppDbContext _context;

        public OmsRepository(AppDbContext context) => _context = context;

        public async Task<OmsPesoPorEdad?> ObtenerPesoPorEdadAsync(char sexo, int edadMeses) =>
            await _context.OmsPesoPorEdad
                .Where(o => o.Sexo == sexo && o.EdadMeses >= edadMeses)
                .OrderBy(o => o.EdadMeses) // Aseguramos que sea el más próximo hacia arriba
                .FirstOrDefaultAsync();

        public async Task<OmsTallaPorEdad?> ObtenerTallaPorEdadAsync(char sexo, int edadMeses) =>
            await _context.OmsTallaPorEdad
                .FirstOrDefaultAsync(o => o.Sexo == sexo && o.EdadMeses == edadMeses);
    }
}

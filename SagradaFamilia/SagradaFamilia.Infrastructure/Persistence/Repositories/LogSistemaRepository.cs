namespace SagradaFamilia.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

public class LogSistemaRepository : ILogSistemaRepository
{
    private readonly AppDbContext _context;

    public LogSistemaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LogSistema>> ObtenerRecientesAsync(int top = 100)
    {
        return await _context.Set<LogSistema>()
            .OrderByDescending(l => l.FechaHora)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogSistema>> ObtenerErroresRecientesAsync(int top)
    {
        return await _context.Set<LogSistema>()
            .Where(l => l.Nivel == "Error" || l.Nivel == "Critical")
            .OrderByDescending(l => l.FechaHora)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogSistema>> ObtenerPorNivelAsync(string nivel)
    {
        return await _context.Set<LogSistema>()
            .Where(l => l.Nivel == nivel)
            .OrderByDescending(l => l.FechaHora)
            .ToListAsync();
    }

    public async Task RegistrarAsync(LogSistema log)
    {
        await _context.Set<LogSistema>().AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
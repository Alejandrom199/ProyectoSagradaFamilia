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

    public async Task<(IEnumerable<LogSistema> Items, int TotalItems)> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var query = _context.Set<LogSistema>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(l =>
                l.Mensaje.ToLower().Contains(term) ||
                l.Nivel.ToLower().Contains(term) ||
                (l.Endpoint != null && l.Endpoint.ToLower().Contains(term)));
        }

        query = sortBy?.ToLower() switch
        {
            "fechahora" => ascending ? query.OrderBy(l => l.FechaHora)    : query.OrderByDescending(l => l.FechaHora),
            "nivel"     => ascending ? query.OrderBy(l => l.Nivel)        : query.OrderByDescending(l => l.Nivel),
            "mensaje"   => ascending ? query.OrderBy(l => l.Mensaje)      : query.OrderByDescending(l => l.Mensaje),
            _           => query.OrderByDescending(l => l.FechaHora)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task RegistrarAsync(LogSistema log)
    {
        await _context.Set<LogSistema>().AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
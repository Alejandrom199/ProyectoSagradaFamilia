using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context) => _context = context;

        public async Task<RefreshToken?> ObtenerPorTokenAsync(string token) =>
            await _context.RefreshTokens
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Token == token);

        public async Task<RefreshToken> CrearAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task RevocarAsync(string token)
        {
            var refreshToken = await ObtenerPorTokenAsync(token);
            if (refreshToken is null) return;

            refreshToken.Revocado = true;
            await _context.SaveChangesAsync();
        }

        public async Task RevocarTodosDeUsuarioAsync(int usuarioId)
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UsuarioId == usuarioId && !r.Revocado)
                .ToListAsync();

            tokens.ForEach(t => t.Revocado = true);
            await _context.SaveChangesAsync();
        }
    }
}

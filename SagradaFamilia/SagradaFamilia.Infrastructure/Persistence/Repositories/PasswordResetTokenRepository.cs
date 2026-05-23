using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetTokenRepository(AppDbContext context) => _context = context;

        public async Task<PasswordResetToken> CrearAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<PasswordResetToken?> ObtenerTokenActivoAsync(string token) =>
            await _context.PasswordResetTokens
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    !t.Usado &&
                    t.FechaExpiracion > DateTime.UtcNow);

        public async Task MarcarUsadoAsync(PasswordResetToken token)
        {
            token.Usado = true;
            token.FechaUso = DateTime.UtcNow;
            _context.PasswordResetTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidarTokensAnterioresAsync(int usuarioId)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UsuarioId == usuarioId && !t.Usado)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.Usado = true;
                t.FechaUso = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}

using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken> CrearAsync(PasswordResetToken token);
        Task<PasswordResetToken?> ObtenerTokenActivoAsync(string token);
        Task MarcarUsadoAsync(PasswordResetToken token);
        Task InvalidarTokensAnterioresAsync(int usuarioId);
    }
}

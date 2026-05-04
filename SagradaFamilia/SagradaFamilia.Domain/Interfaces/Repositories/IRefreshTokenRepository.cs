using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> ObtenerPorTokenAsync(string token);
        Task<RefreshToken> CrearAsync(RefreshToken refreshToken);
        Task RevocarAsync(string token);
        Task RevocarTodosDeUsuarioAsync(int usuarioId);
    }
}

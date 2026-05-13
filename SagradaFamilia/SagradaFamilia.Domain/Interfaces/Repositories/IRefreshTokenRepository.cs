using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Busca un token específico. Fundamental para el proceso de "Refresh" 
        /// donde Angular envía el token viejo para obtener uno nuevo.
        /// </summary>
        Task<RefreshToken?> ObtenerPorTokenAsync(string token);

        /// <summary>
        /// Registra un nuevo token de refresco vinculado a un usuario.
        /// </summary>
        Task<RefreshToken> CrearAsync(RefreshToken refreshToken);

        /// <summary>
        /// Marca un token como inválido. Se usa en el Logout individual.
        /// </summary>
        Task RevocarAsync(string token);

        /// <summary>
        /// Invalida todas las sesiones de un usuario. 
        /// Medida de seguridad vital si el usuario cambia su contraseña 
        /// o si reporta un acceso no autorizado.
        /// </summary>
        Task RevocarTodosDeUsuarioAsync(int usuarioId);
    }
}

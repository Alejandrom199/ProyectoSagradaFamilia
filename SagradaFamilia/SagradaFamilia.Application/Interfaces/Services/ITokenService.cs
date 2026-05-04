using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerarAccessToken(Usuario usuario);
        string GenerarRefreshToken();
    }

}

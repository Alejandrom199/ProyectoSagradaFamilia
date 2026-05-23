namespace SagradaFamilia.Application.Interfaces.Services;

using SagradaFamilia.Application.DTOs.Auth;

public interface IAuthService
{
    Task<LoginDto.Response> LoginAsync(LoginDto.Request request);
    Task<LoginDto.Response> RefreshTokenAsync(RefreshTokenDto.Request request);
    Task NuevaClaveAsync(NuevaClaveDto.Request request);
}
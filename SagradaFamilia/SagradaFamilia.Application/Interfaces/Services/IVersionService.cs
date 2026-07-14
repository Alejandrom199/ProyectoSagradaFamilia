using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IVersionService
    {
        Task<VersionDto> ObtenerAsync();
    }
}

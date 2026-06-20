using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IDashboardAdminService
    {
        Task<SistemaDashboardDto> ObtenerAsync();
    }
}

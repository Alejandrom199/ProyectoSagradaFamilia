using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IRolService
    {
        Task<IEnumerable<RolDto.Response>> ObtenerTodosAsync();
        Task<RolDto.Response?> ObtenerPorIdAsync(int id);
    }
}
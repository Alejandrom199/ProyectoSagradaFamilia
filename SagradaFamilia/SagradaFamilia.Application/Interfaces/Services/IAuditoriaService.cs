using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IAuditoriaService
    {
        Task<IEnumerable<AuditoriaDto.Response>> ObtenerRecientesAsync(int top = 100);
        Task<IEnumerable<AuditoriaDto.Response>> ObtenerPorTablaAsync(string nombreTabla, string? clavePrimaria = null);
        Task<IEnumerable<AuditoriaDto.Response>> ObtenerPorUsuarioAsync(int usuarioId);
        Task RegistrarAsync(AuditoriaDto.Create request);
    }
}
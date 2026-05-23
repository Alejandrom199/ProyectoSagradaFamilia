using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface ILogSistemaService
    {
        Task<IEnumerable<LogSistemaDto.Response>> ObtenerRecientesAsync(int top = 100);
        Task<IEnumerable<LogSistemaDto.Response>> ObtenerErroresRecientesAsync(int top = 50);
        Task<IEnumerable<LogSistemaDto.Response>> ObtenerPorNivelAsync(string nivel);
        Task RegistrarEventoAsync(string nivel, string mensaje, string endpoint, int? usuarioId = null);
    }
}
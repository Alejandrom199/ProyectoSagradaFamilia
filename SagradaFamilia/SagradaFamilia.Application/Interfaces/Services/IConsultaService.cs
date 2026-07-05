using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IConsultaService
    {
        Task<ConsultaDto.Response> IniciarAsync(int citaId, int usuarioId);
        Task<ConsultaDto.Response> ActualizarAsync(int id, ConsultaDto.Actualizar request, int usuarioId);
        Task<ConsultaDto.Response> CompletarAsync(int id, int usuarioId);
        Task<IEnumerable<ConsultaDto.Response>> ObtenerHistorialPorNinoAsync(int ninoId);
    }
}

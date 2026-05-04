using SagradaFamilia.Application.DTOs.Medidas;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IMedidaService
    {
        Task<IEnumerable<MedidaResponse>> ObtenerPorNinoAsync(int ninoId);
        Task<MedidaResponse> CrearAsync(CrearMedidaRequest request, int medicoId);
        Task<MedidaResponse> ActualizarAsync(int id, ActualizarMedidaRequest request);
        Task EliminarAsync(int id);
    }
}

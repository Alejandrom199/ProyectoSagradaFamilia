using SagradaFamilia.Application.DTOs.Predicciones;

namespace SagradaFamilia.Application.Interfaces.Services.External
{
    public interface IProphetApiClient
    {
        Task<PrediccionResponse> PredecirPesoAsync(
            int ninoId,
            char sexo,
            List<(DateOnly Fecha, decimal Peso)> medidas,
            decimal cap,
            decimal floor
        );

        Task<PrediccionHealth> EstadoServicioPredecirAsync();
    }
}

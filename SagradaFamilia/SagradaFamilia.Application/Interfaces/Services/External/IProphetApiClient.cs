using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services.External;

public interface IProphetApiClient
{
    Task<PrediccionDto.Response> PredecirPesoAsync(
        int ninoId,
        char sexo,
        List<(DateOnly Fecha, decimal Peso)> medidas,
        decimal cap,
        decimal floor);

    Task<PrediccionDto.Health> EstadoServicioPredecirAsync();
}

using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services.External;

public interface IProphetApiClient
{
    Task<PrediccionDto.Response> PredecirPesoAsync(int edadMeses, char sexo, List<(DateOnly Fecha, decimal Peso)> historico, decimal pesoActual, decimal tallaActual);
    Task<PrediccionDto.Health> EstadoServicioPredecirAsync();
}
using System.Net.Http.Json;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services.External;

namespace SagradaFamilia.Infrastructure.ExternalServices;

public class ProphetApiClient : IProphetApiClient
{
    private readonly HttpClient _httpClient;

    public ProphetApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<PrediccionDto.Health> EstadoServicioPredecirAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<PrediccionDto.Health>("health");
        return response ?? new PrediccionDto.Health { Status = "error", Service = "Prophet API" };
    }

    public async Task<PrediccionDto.Response> PredecirPesoAsync(int edadMeses, char sexo, List<(DateOnly Fecha, decimal Peso)> historico, decimal pesoActual, decimal tallaActual)
    {
        var request = new
        {
            edad_meses = edadMeses,
            sexo = sexo,
            historico = historico.Select(h => new { fecha = h.Fecha, peso = h.Peso }),
            peso_actual = pesoActual,
            talla_actual = tallaActual
        };

        var response = await _httpClient.PostAsJsonAsync("predict", request);

        if (!response.IsSuccessStatusCode)
            return new PrediccionDto.Response { PuedePredecir = false, Mensaje = "Error al conectar con el motor de IA." };

        return await response.Content.ReadFromJsonAsync<PrediccionDto.Response>() ?? new PrediccionDto.Response();
    }
}
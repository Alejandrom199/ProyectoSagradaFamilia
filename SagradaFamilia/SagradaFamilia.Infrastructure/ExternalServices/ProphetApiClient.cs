using System.Net.Http.Json;
using System.Text.Json.Serialization;
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

    public async Task<PrediccionDto.Response> PredecirPesoAsync(
        int ninoId,
        char sexo,
        List<(DateOnly Fecha, decimal Peso)> medidas,
        decimal cap,
        decimal floor)
    {
        var request = new
        {
            nino_id = ninoId,
            sexo    = sexo.ToString(),
            medidas = medidas.Select(m => new
            {
                fecha = m.Fecha.ToString("yyyy-MM-dd"),
                peso  = (double)m.Peso
            }),
            cap   = (double)cap,
            floor = (double)floor
        };

        var response = await _httpClient.PostAsJsonAsync("api/prediccion/peso", request);

        if (!response.IsSuccessStatusCode)
            return new PrediccionDto.Response
            {
                PuedePredecir = false,
                NinoId        = ninoId,
                Mensaje       = "Error al conectar con el motor de predicción."
            };

        var raw = await response.Content.ReadFromJsonAsync<RawResponse>();
        if (raw == null)
            return new PrediccionDto.Response { PuedePredecir = false, NinoId = ninoId };

        return new PrediccionDto.Response
        {
            PuedePredecir = raw.PuedePredecir,
            NinoId        = raw.NinoId,
            Mensaje       = raw.Mensaje,
            Predicciones  = (raw.Predicciones ?? []).Select(p => new PrediccionDto.Punto
            {
                Meses          = p.Meses,
                FechaObjetivo  = DateOnly.Parse(p.Fecha),
                PesoPredicho   = (decimal)p.Predicho,
                PesoMinimo     = (decimal)p.Minimo,
                PesoMaximo     = (decimal)p.Maximo
            }).ToList()
        };
    }

    // DTOs internos para deserializar la respuesta snake_case de la API Python
    private sealed record RawResponse(
        [property: JsonPropertyName("puede_predecir")] bool             PuedePredecir,
        [property: JsonPropertyName("nino_id")]        int              NinoId,
        [property: JsonPropertyName("mensaje")]        string?          Mensaje,
        [property: JsonPropertyName("predicciones")]   List<RawPunto>?  Predicciones);

    private sealed record RawPunto(
        [property: JsonPropertyName("meses")]    int    Meses,
        [property: JsonPropertyName("fecha")]    string Fecha,
        [property: JsonPropertyName("predicho")] double Predicho,
        [property: JsonPropertyName("minimo")]   double Minimo,
        [property: JsonPropertyName("maximo")]   double Maximo);
}

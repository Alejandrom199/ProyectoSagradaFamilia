using Microsoft.Extensions.Configuration;
using SagradaFamilia.Application.DTOs.Predicciones;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SagradaFamilia.Infrastructure.ExternalServices
{
    public class ProphetApiClient : IProphetApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProphetApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(
                _configuration["ProphetApi:BaseUrl"] ?? "http://localhost:8000");
        }

        public async Task<PrediccionResponse> PredecirPesoAsync(
            int ninoId,
            char sexo,
            List<(DateOnly Fecha, decimal Peso)> medidas,
            decimal cap,
            decimal floor)
        {
            var payload = new
            {
                nino_id = ninoId,
                sexo = sexo.ToString(),
                cap,
                floor,
                medidas = medidas.Select(m => new
                {
                    fecha = m.Fecha.ToString("yyyy-MM-dd"),
                    peso = m.Peso
                })
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/prediccion/peso", payload);
                response.EnsureSuccessStatusCode();

                var resultado = await response.Content.ReadFromJsonAsync<ProphetApiResponse>();

                if (resultado is null)
                    return RespuestaError(ninoId, "Error al deserializar la respuesta de Prophet.");

                return new PrediccionResponse
                {
                    PuedePredecir = resultado.PuedePredecir,
                    NinoId = ninoId,
                    Mensaje = resultado.Mensaje,
                    Predicciones = resultado.Predicciones.Select(p => new PuntoPrediccion
                    {
                        Meses = p.Meses,
                        FechaObjetivo = DateOnly.Parse(p.Fecha),
                        PesoPredicho = p.Predicho,
                        PesoMinimo = p.Minimo,
                        PesoMaximo = p.Maximo
                    }).ToList()
                };
            }
            catch (HttpRequestException)
            {
                return RespuestaError(ninoId, "El servicio de predicción no está disponible.");
            }
        }

        public async Task<PrediccionHealth> EstadoServicioPredecirAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/health");
                response.EnsureSuccessStatusCode();

                var resultado = await response.Content.ReadFromJsonAsync<ProphetHealtResponse>();

                if (resultado is null)
                    return RespuestaError( "Error al deserializar la respuesta del servicio.");

                return new PrediccionHealth
                {
                    Service = resultado.Service,
                    Status = resultado.Status,
                    Version = resultado.Version
                };
            }
            catch (HttpRequestException)
            {
                return RespuestaError("El servicio de predicción no está disponible.");
            }
        }

        private sealed class ProphetApiResponse
        {
            [JsonPropertyName("puede_predecir")]
            public bool PuedePredecir { get; set; }

            [JsonPropertyName("mensaje")]
            public string? Mensaje { get; set; }

            [JsonPropertyName("nino_id")]
            public int NinoId { get; set; }

            [JsonPropertyName("predicciones")]
            public List<ProphetPunto> Predicciones { get; set; } = new();
        }

        private sealed class ProphetPunto
        {
            [JsonPropertyName("meses")]
            public int Meses { get; set; }

            [JsonPropertyName("fecha")]
            public string Fecha { get; set; } = string.Empty;

            [JsonPropertyName("predicho")]
            public decimal Predicho { get; set; }

            [JsonPropertyName("minimo")]
            public decimal Minimo { get; set; }

            [JsonPropertyName("maximo")]
            public decimal Maximo { get; set; }
        }

        private sealed class ProphetHealtResponse
        {
            [JsonPropertyName("service")]
            public string Service { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("version")]
            public string? Version { get; set; }
        }

        private static PrediccionResponse RespuestaError(int ninoId, string mensaje) =>
            new() { PuedePredecir = false, NinoId = ninoId, Mensaje = mensaje };
        private static PrediccionHealth RespuestaError(string service) =>
            new() { Service = service };
    }
}

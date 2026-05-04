namespace SagradaFamilia.Application.Services;

using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Predicciones;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class PrediccionService : IPrediccionService
{
    private readonly INinoRepository _ninoRepository;
    private readonly IMedidaRepository _medidaRepository;
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IOmsRepository _omsRepository;
    private readonly IProphetApiClient _prophetApiClient;
    private readonly ILogger<PrediccionService> _logger;

    public PrediccionService(
        INinoRepository ninoRepository,
        IMedidaRepository medidaRepository,
        IPrediccionRepository prediccionRepository,
        IOmsRepository omsRepository,
        IProphetApiClient prophetApiClient,
        ILogger<PrediccionService> logger)
    {
        _ninoRepository = ninoRepository;
        _medidaRepository = medidaRepository;
        _prediccionRepository = prediccionRepository;
        _omsRepository = omsRepository;
        _prophetApiClient = prophetApiClient;
        _logger = logger;
    }

    public async Task<PrediccionResponse> ObtenerPrediccionesAsync(int ninoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
            ?? throw new NotFoundException("Niño", ninoId);

        var medidas = (await _medidaRepository.ObtenerPorNinoAsync(ninoId))
            .Where(m => !m.Eliminado)
            .OrderBy(m => m.FechaMedicion)
            .ToList();

        if (medidas.Count < 3)
        {
            _logger.LogWarning(
                "Niño ID {Id} tiene solo {Count} medidas, mínimo 3 requeridas",
                ninoId, medidas.Count);

            return new PrediccionResponse
            {
                PuedePredecir = false,
                NinoId = ninoId,
                Mensaje = $"Se necesitan al menos 3 medidas. Hay {medidas.Count}."
            };
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var edadActualMeses = CalcularEdadMeses(nino.FechaNacimiento, hoy);
        var edadProyectadaMeses = Math.Min(edadActualMeses + 12, 60);

        var omsProyectado = await _omsRepository
            .ObtenerPesoPorEdadAsync(nino.Sexo, edadProyectadaMeses);

        var omsActual = await _omsRepository
            .ObtenerPesoPorEdadAsync(nino.Sexo, edadActualMeses);

        var cap = omsProyectado?.Percentil97 ?? 22.0m;
        var floor = omsActual?.Percentil3 ?? 2.5m;

        // Una medida por mes — toma la más reciente de cada mes
        var medidasParaProphet = medidas
            .GroupBy(m => new { m.FechaMedicion.Year, m.FechaMedicion.Month })
            .Select(g => g.OrderByDescending(m => m.FechaMedicion).First())
            .Select(m => (m.FechaMedicion, m.Peso))
            .ToList();

        _logger.LogInformation(
            "Llamando a Prophet para niño ID: {Id} con {Count} medidas",
            ninoId, medidasParaProphet.Count);

        var resultado = await _prophetApiClient.PredecirPesoAsync(
            ninoId, nino.Sexo, medidasParaProphet, cap, floor);

        if (!resultado.PuedePredecir)
            return resultado;

        var predicciones = resultado.Predicciones.Select(p => new Prediccion
        {
            NinoId = ninoId,
            FechaObjetivo = p.FechaObjetivo,
            Meses = p.Meses,
            PesoPredicho = p.PesoPredicho,
            PesoMinimo = p.PesoMinimo,
            PesoMaximo = p.PesoMaximo
        });

        await _prediccionRepository.GuardarPrediccionesAsync(predicciones);

        _logger.LogInformation("Predicciones guardadas para niño ID: {Id}", ninoId);

        return resultado;
    }

    public async Task<PrediccionHealth> ObtenerEstadoServicioPrediccionAsync()
    {
        _logger.LogInformation("Servicio de predicción no disponible");

        var resultado = await _prophetApiClient.EstadoServicioPredecirAsync();

        return resultado;
    }
    private static int CalcularEdadMeses(DateOnly fechaNacimiento, DateOnly fechaReferencia)
    {
        var meses = ((fechaReferencia.Year - fechaNacimiento.Year) * 12)
                  + fechaReferencia.Month - fechaNacimiento.Month;

        if (fechaReferencia.Day < fechaNacimiento.Day)
            meses--;

        return Math.Max(0, meses);
    }
}
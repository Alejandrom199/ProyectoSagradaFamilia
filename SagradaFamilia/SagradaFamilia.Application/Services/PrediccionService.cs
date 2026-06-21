using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Services;

public class PrediccionService : IPrediccionService
{
    private readonly INinoRepository       _ninoRepository;
    private readonly IMedidaRepository     _medidaRepository;
    private readonly IOmsRepository        _omsRepository;
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IProphetApiClient     _prophetClient;

    public PrediccionService(
        INinoRepository       ninoRepository,
        IMedidaRepository     medidaRepository,
        IOmsRepository        omsRepository,
        IPrediccionRepository prediccionRepository,
        IProphetApiClient     prophetClient)
    {
        _ninoRepository       = ninoRepository;
        _medidaRepository     = medidaRepository;
        _omsRepository        = omsRepository;
        _prediccionRepository = prediccionRepository;
        _prophetClient        = prophetClient;
    }

    public async Task<PrediccionDto.Response> ObtenerPrediccionesAsync(int ninoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
                   ?? throw new NotFoundException("Niño", ninoId);

        var medidas = (await _medidaRepository.ObtenerPorNinoAsync(ninoId)).ToList();

        if (medidas.Count < 3)
        {
            return new PrediccionDto.Response
            {
                PuedePredecir = false,
                NinoId        = ninoId,
                Mensaje       = "Se requieren al menos 3 medidas históricas para generar una predicción confiable."
            };
        }

        int edadMeses = ((DateTime.Now.Year - nino.FechaNacimiento.Year) * 12)
                      + DateTime.Now.Month - nino.FechaNacimiento.Month;

        var oms   = await _omsRepository.ObtenerReferenciaAsync(nino.Sexo, edadMeses, TipoReferencia.Peso);
        decimal cap   = oms?.Percentil97 ?? 22.0m;
        decimal floor = oms?.Percentil3  ?? 2.5m;

        var historico = medidas
            .Select(m => (m.FechaMedicion, m.Peso))
            .ToList();

        var resultado = await _prophetClient.PredecirPesoAsync(
            ninoId, nino.Sexo, historico, cap, floor);

        // Persistir predicciones y completar ValorReal con medidas ya existentes
        if (resultado.PuedePredecir && resultado.Predicciones.Any())
        {
            var prediccionesEntidad = resultado.Predicciones.Select(p => new Prediccion
            {
                NinoId        = ninoId,
                FechaCalculo  = DateTime.UtcNow,
                FechaObjetivo = p.FechaObjetivo,
                ProyeccionMeses = p.Meses,
                Tipo          = TipoReferencia.Peso,
                ValorPredicho = p.PesoPredicho,
                ValorMinimo   = p.PesoMinimo,
                ValorMaximo   = p.PesoMaximo,
            }).ToList();

            await _prediccionRepository.GuardarPrediccionesAsync(prediccionesEntidad);

            // Completar ValorReal si ya hay medidas en esas fechas
            foreach (var punto in resultado.Predicciones)
            {
                var medidaReal = medidas.FirstOrDefault(m =>
                    m.FechaMedicion.Year  == punto.FechaObjetivo.Year &&
                    m.FechaMedicion.Month == punto.FechaObjetivo.Month);

                if (medidaReal is not null)
                    punto.PesoReal = medidaReal.Peso;
            }
        }

        return resultado;
    }

    public async Task<PrediccionDto.CurvasOms> ObtenerCurvasOmsAsync(int ninoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
                   ?? throw new NotFoundException("Niño", ninoId);

        var referencias = await _omsRepository.ObtenerCurvaCompletaAsync(nino.Sexo, TipoReferencia.Peso);

        return new PrediccionDto.CurvasOms
        {
            Sexo  = nino.Sexo,
            Tipo  = "Peso",
            Curvas = referencias.Select(r => new PrediccionDto.PuntoOms
            {
                EdadMeses   = r.EdadMeses,
                Percentil3  = r.Percentil3,
                Percentil15 = r.Percentil15,
                Percentil50 = r.Percentil50,
                Percentil85 = r.Percentil85,
                Percentil97 = r.Percentil97,
            }).ToList()
        };
    }

    public async Task<PrediccionDto.Health> ObtenerEstadoServicioPrediccionAsync()
    {
        return await _prophetClient.EstadoServicioPredecirAsync();
    }

    public async Task<int> ContarNinosConPrediccionAsync() =>
        await _prediccionRepository.ContarNinosConPrediccionAsync();
}

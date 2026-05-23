using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Services;

public class PrediccionService : IPrediccionService
{
    private readonly INinoRepository    _ninoRepository;
    private readonly IMedidaRepository  _medidaRepository;
    private readonly IOmsRepository     _omsRepository;
    private readonly IProphetApiClient  _prophetClient;

    public PrediccionService(
        INinoRepository   ninoRepository,
        IMedidaRepository medidaRepository,
        IOmsRepository    omsRepository,
        IProphetApiClient prophetClient)
    {
        _ninoRepository   = ninoRepository;
        _medidaRepository = medidaRepository;
        _omsRepository    = omsRepository;
        _prophetClient    = prophetClient;
    }

    public async Task<PrediccionDto.Response> ObtenerPrediccionesAsync(int ninoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
                   ?? throw new NotFoundException("Niño", ninoId);

        var medidas = await _medidaRepository.ObtenerPorNinoAsync(ninoId);

        if (medidas.Count() < 3)
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

        // Límites biológicos de la tabla OMS; si no hay datos se usan los defaults de Prophet
        var oms   = await _omsRepository.ObtenerReferenciaAsync(nino.Sexo, edadMeses, TipoReferencia.Peso);
        decimal cap   = oms?.Percentil97 ?? 22.0m;
        decimal floor = oms?.Percentil3  ?? 2.5m;

        var historico = medidas
            .Select(m => (m.FechaMedicion, m.Peso))
            .ToList();

        return await _prophetClient.PredecirPesoAsync(
            ninoId,
            nino.Sexo,
            historico,
            cap,
            floor);
    }

    public async Task<PrediccionDto.Health> ObtenerEstadoServicioPrediccionAsync()
    {
        return await _prophetClient.EstadoServicioPredecirAsync();
    }
}

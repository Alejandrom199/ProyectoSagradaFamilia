namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class MedidaService : IMedidaService
{
    private readonly IMedidaRepository    _medidaRepository;
    private readonly INinoRepository      _ninoRepository;
    private readonly IOmsRepository       _omsRepository;
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IMapper              _mapper;
    private readonly ILogger<MedidaService> _logger;

    public MedidaService(
        IMedidaRepository     medidaRepository,
        INinoRepository       ninoRepository,
        IOmsRepository        omsRepository,
        IPrediccionRepository prediccionRepository,
        IMapper               mapper,
        ILogger<MedidaService> logger)
    {
        _medidaRepository    = medidaRepository;
        _ninoRepository      = ninoRepository;
        _omsRepository       = omsRepository;
        _prediccionRepository = prediccionRepository;
        _mapper              = mapper;
        _logger              = logger;
    }

    public async Task<MedidaDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Consultando medida ID: {Id}", id);

        var medida = await _medidaRepository.ObtenerPorIdAsync(id)
                     ?? throw new NotFoundException("Medida", id);

        var response = _mapper.Map<MedidaDto.Response>(medida);
        await EnriquecerConDatosOms(response, medida.Nino, medida.FechaMedicion, medida.Peso, medida.Talla);

        return response;
    }

    public async Task<IEnumerable<MedidaDto.Response>> ObtenerPorNinoAsync(int ninoId)
    {
        _logger.LogInformation("Obteniendo historial de medidas para el niño ID: {NinoId}", ninoId);

        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
                   ?? throw new NotFoundException("Niño", ninoId);

        var medidas = await _medidaRepository.ObtenerPorNinoAsync(ninoId);
        var lista   = new List<MedidaDto.Response>();

        foreach (var m in medidas)
        {
            var res = _mapper.Map<MedidaDto.Response>(m);
            await EnriquecerConDatosOms(res, nino, m.FechaMedicion, m.Peso, m.Talla);
            lista.Add(res);
        }

        return lista;
    }

    public async Task<MedidaDto.Response?> ObtenerUltimaMedidaAsync(int ninoId)
    {
        _logger.LogInformation("Buscando última medida para el niño ID: {NinoId}", ninoId);

        var medida = await _medidaRepository.ObtenerUltimaMedidaAsync(ninoId);
        if (medida == null) return null;

        var nino     = await _ninoRepository.ObtenerPorIdAsync(ninoId);
        var response = _mapper.Map<MedidaDto.Response>(medida);

        if (nino != null)
            await EnriquecerConDatosOms(response, nino, medida.FechaMedicion, medida.Peso, medida.Talla);

        return response;
    }

    public async Task<MedidaDto.Response> CrearAsync(MedidaDto.Create request, int medicoId)
    {
        _logger.LogInformation("Creando medida para Niño ID: {NinoId}", request.NinoId);

        var nino = await _ninoRepository.ObtenerPorIdAsync(request.NinoId)
                   ?? throw new NotFoundException("Niño", request.NinoId);

        if (await _medidaRepository.ExisteMedidaEnMesAsync(
                request.NinoId, request.FechaMedicion.Month, request.FechaMedicion.Year))
        {
            throw new BusinessException("Ya existe una medida registrada para este niño en el mes seleccionado.");
        }

        var medida  = _mapper.Map<Medida>(request);
        medida.MedicoId = medicoId;

        var creada = await _medidaRepository.CrearAsync(medida);

        await _prediccionRepository.ActualizarValorRealAsync(
            request.NinoId, request.FechaMedicion, request.Peso, TipoReferencia.Peso);

        _logger.LogInformation("Medida ID {Id} creada y modelo de predicción actualizado.", creada.Id);

        var response = _mapper.Map<MedidaDto.Response>(creada);
        await EnriquecerConDatosOms(response, nino, creada.FechaMedicion, creada.Peso, creada.Talla);

        return response;
    }

    public async Task<MedidaDto.Response> ActualizarAsync(int id, MedidaDto.Update request)
    {
        _logger.LogInformation("Actualizando medida ID: {Id}", id);

        var medida = await _medidaRepository.ObtenerPorIdAsync(id)
                     ?? throw new NotFoundException("Medida", id);

        _mapper.Map(request, medida);
        var actualizada = await _medidaRepository.ActualizarAsync(medida);

        var nino     = await _ninoRepository.ObtenerPorIdAsync(actualizada.NinoId);
        var response = _mapper.Map<MedidaDto.Response>(actualizada);

        if (nino != null)
            await EnriquecerConDatosOms(response, nino, actualizada.FechaMedicion, actualizada.Peso, actualizada.Talla);

        return response;
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Eliminando medida ID: {Id}", id);

        var medida = await _medidaRepository.ObtenerPorIdAsync(id)
                     ?? throw new NotFoundException("Medida", id);

        await _medidaRepository.EliminarAsync(medida.Id);
        _logger.LogInformation("Medida ID: {Id} eliminada correctamente.", id);
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private async Task EnriquecerConDatosOms(
        MedidaDto.Response res,
        Nino   nino,
        DateOnly fechaMedida,
        decimal pesoActual,
        decimal tallaActual)
    {
        var edadMeses = CalcularEdadMeses(nino.FechaNacimiento, fechaMedida);

        var refPeso = await _omsRepository.ObtenerReferenciaAsync(nino.Sexo, edadMeses, TipoReferencia.Peso);
        if (refPeso != null)
        {
            res.EstadoNutricional = DeterminarEstado(pesoActual, refPeso).ToString();
            res.PercentilPeso     = CalcularPercentil(pesoActual, refPeso);
        }

        var refTalla = await _omsRepository.ObtenerReferenciaAsync(nino.Sexo, edadMeses, TipoReferencia.Talla);
        if (refTalla != null)
            res.PercentilTalla = CalcularPercentil(tallaActual, refTalla);
    }

    private static int CalcularEdadMeses(DateOnly nacimiento, DateOnly medida)
    {
        var meses = ((medida.Year - nacimiento.Year) * 12) + medida.Month - nacimiento.Month;
        if (medida.Day < nacimiento.Day) meses--;
        return Math.Max(0, meses);
    }

    private static EstadoNutricional DeterminarEstado(decimal peso, OmsReferencia oms) => peso switch
    {
        _ when peso < oms.Percentil3  => EstadoNutricional.BajoPesoSevero,
        _ when peso < oms.Percentil15 => EstadoNutricional.BajoPeso,
        _ when peso < oms.Percentil85 => EstadoNutricional.Normal,
        _ when peso < oms.Percentil97 => EstadoNutricional.Sobrepeso,
        _                             => EstadoNutricional.Obesidad
    };

    private static int CalcularPercentil(decimal valor, OmsReferencia oms)
    {
        if (valor <= oms.Percentil3)  return 3;
        if (valor <= oms.Percentil15) return 15;
        if (valor <= oms.Percentil50) return 50;
        if (valor <= oms.Percentil85) return 85;
        if (valor <  oms.Percentil97) return 90;
        return 97;
    }
}

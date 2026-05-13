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
    private readonly IMedidaRepository _medidaRepository;
    private readonly INinoRepository _ninoRepository;
    private readonly IOmsRepository _omsRepository;
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MedidaService> _logger;

    public MedidaService(
        IMedidaRepository medidaRepository,
        INinoRepository ninoRepository,
        IOmsRepository omsRepository,
        IPrediccionRepository prediccionRepository,
        IMapper mapper,
        ILogger<MedidaService> logger)
    {
        _medidaRepository = medidaRepository;
        _ninoRepository = ninoRepository;
        _omsRepository = omsRepository;
        _prediccionRepository = prediccionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MedidaDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Consultando medida ID: {Id}", id);

        var medida = await _medidaRepository.ObtenerPorIdAsync(id);
        if (medida == null)
        {
            _logger.LogError("Error: Medida con ID {Id} no encontrada.", id);
            throw new NotFoundException("Medida", id);
        }

        var response = _mapper.Map<MedidaDto.Response>(medida);
        await EnriquecerConDatosOms(response, medida.Nino, medida.FechaMedicion, medida.Peso);

        return response;
    }

    public async Task<IEnumerable<MedidaDto.Response>> ObtenerPorNinoAsync(int ninoId)
    {
        _logger.LogInformation("Obteniendo historial de medidas para el niño ID: {NinoId}", ninoId);

        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
        if (nino == null)
        {
            _logger.LogError("No se puede obtener historial: El niño ID {Id} no existe.", ninoId);
            throw new NotFoundException("Niño", ninoId);
        }

        var medidas = await _medidaRepository.ObtenerPorNinoAsync(ninoId);
        var listaResponse = new List<MedidaDto.Response>();

        foreach (var m in medidas)
        {
            var res = _mapper.Map<MedidaDto.Response>(m);
            await EnriquecerConDatosOms(res, nino, m.FechaMedicion, m.Peso);
            listaResponse.Add(res);
        }

        return listaResponse;
    }

    public async Task<MedidaDto.Response?> ObtenerUltimaMedidaAsync(int ninoId)
    {
        _logger.LogInformation("Buscando última medida para el niño ID: {NinoId}", ninoId);

        var medida = await _medidaRepository.ObtenerUltimaMedidaAsync(ninoId);
        if (medida == null) return null;

        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
        var response = _mapper.Map<MedidaDto.Response>(medida);

        if (nino != null)
            await EnriquecerConDatosOms(response, nino, medida.FechaMedicion, medida.Peso);

        return response;
    }

    public async Task<MedidaDto.Response> CrearAsync(MedidaDto.Create request, int medicoId)
    {
        _logger.LogInformation("Creando medida para Niño ID: {NinoId}", request.NinoId);

        var nino = await _ninoRepository.ObtenerPorIdAsync(request.NinoId);
        if (nino == null)
        {
            _logger.LogError("Error en creación: Niño ID {Id} no existe.", request.NinoId);
            throw new NotFoundException("Niño", request.NinoId);
        }

        if (await _medidaRepository.ExisteMedidaEnMesAsync(request.NinoId, request.FechaMedicion.Month, request.FechaMedicion.Year))
        {
            _logger.LogWarning("Validación fallida: Ya existe una medida en el mes {Mes}/{Anio}",
                request.FechaMedicion.Month, request.FechaMedicion.Year);
            throw new BusinessException("Ya existe una medida registrada para este niño en el mes seleccionado.");
        }

        var medida = _mapper.Map<Medida>(request);
        medida.MedicoId = medicoId;

        var creada = await _medidaRepository.CrearAsync(medida);

        await _prediccionRepository.ActualizarValorRealAsync(
            request.NinoId,
            request.FechaMedicion,
            request.Peso,
            TipoReferencia.Peso);

        _logger.LogInformation("Medida ID {Id} creada y modelo de predicción actualizado.", creada.Id);

        var response = _mapper.Map<MedidaDto.Response>(creada);
        await EnriquecerConDatosOms(response, nino, creada.FechaMedicion, creada.Peso);

        return response;
    }

    public async Task<MedidaDto.Response> ActualizarAsync(int id, MedidaDto.Update request)
    {
        _logger.LogInformation("Actualizando medida ID: {Id}", id);

        var medida = await _medidaRepository.ObtenerPorIdAsync(id);
        if (medida == null)
        {
            _logger.LogError("Error en actualización: Medida ID {Id} no encontrada.", id);
            throw new NotFoundException("Medida", id);
        }

        _mapper.Map(request, medida);
        var actualizada = await _medidaRepository.ActualizarAsync(medida);

        var nino = await _ninoRepository.ObtenerPorIdAsync(actualizada.NinoId);
        var response = _mapper.Map<MedidaDto.Response>(actualizada);

        if (nino != null)
            await EnriquecerConDatosOms(response, nino, actualizada.FechaMedicion, actualizada.Peso);

        return response;
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Eliminando medida ID: {Id}", id);

        var medida = await _medidaRepository.ObtenerPorIdAsync(id);
        if (medida == null)
        {
            _logger.LogError("Fallo al eliminar: Medida ID {Id} no encontrada.", id);
            throw new NotFoundException("Medida", id);
        }

        await _medidaRepository.EliminarAsync(medida.Id);
        _logger.LogInformation("Medida ID: {Id} eliminada correctamente.", id);
    }

    private async Task EnriquecerConDatosOms(MedidaDto.Response res, Nino nino, DateOnly fechaMedida, decimal pesoActual)
    {
        var edadMeses = CalcularEdadMeses(nino.FechaNacimiento, fechaMedida);

        var referencia = await _omsRepository.ObtenerReferenciaAsync(nino.Sexo, edadMeses, TipoReferencia.Peso);

        if (referencia != null)
        {
            res.EstadoNutricional = DeterminarEstado(pesoActual, referencia).ToString();
            res.PercentilPeso = CalcularPercentil(pesoActual, referencia);
        }
    }

    private static int CalcularEdadMeses(DateOnly nacimiento, DateOnly medida)
    {
        var meses = ((medida.Year - nacimiento.Year) * 12) + medida.Month - nacimiento.Month;
        if (medida.Day < nacimiento.Day) meses--;
        return Math.Max(0, meses);
    }

    private static EstadoNutricional DeterminarEstado(decimal peso, OmsReferencia oms) => peso switch
    {
        _ when peso < oms.Percentil3 => EstadoNutricional.BajoPesoSevero,
        _ when peso < oms.Percentil15 => EstadoNutricional.BajoPeso,
        _ when peso < oms.Percentil85 => EstadoNutricional.Normal,
        _ when peso < oms.Percentil97 => EstadoNutricional.Sobrepeso,
        _ => EstadoNutricional.Obesidad
    };

    // 💡 Corregido: Uso de OmsReferencia
    private static int CalcularPercentil(decimal peso, OmsReferencia oms)
    {
        if (peso <= oms.Percentil3) return 3;
        if (peso >= oms.Percentil97) return 97;
        if (peso <= oms.Percentil15) return 15;
        if (peso <= oms.Percentil50) return 50;
        if (peso <= oms.Percentil85) return 85;
        return 90;
    }
}
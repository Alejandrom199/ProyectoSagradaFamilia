namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Medidas;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
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

    public async Task<IEnumerable<MedidaResponse>> ObtenerPorNinoAsync(int ninoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
            ?? throw new NotFoundException("Niño", ninoId);

        var medidas = await _medidaRepository.ObtenerPorNinoAsync(ninoId);
        var responses = new List<MedidaResponse>();

        foreach (var medida in medidas)
        {
            var response = _mapper.Map<MedidaResponse>(medida);
            var edadMeses = CalcularEdadMeses(nino.FechaNacimiento, medida.FechaMedicion);
            var oms = await _omsRepository.ObtenerPesoPorEdadAsync(nino.Sexo, edadMeses);

            if (oms is not null)
            {
                response.EstadoNutricional = DeterminarEstadoNutricional(medida.Peso, oms).ToString();
                response.Percentil = EstimarPercentil(medida.Peso, oms);
            }

            responses.Add(response);
        }

        return responses;
    }

    public async Task<MedidaResponse> CrearAsync(CrearMedidaRequest request, int medicoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(request.NinoId)
            ?? throw new NotFoundException("Niño", request.NinoId);

        if (await _medidaRepository.ExisteMedidaEnMesAsync(request.NinoId, request.FechaMedicion))
            throw new BusinessException(
                "Ya existe una medida registrada para este niño en ese mes.");

        var medida = _mapper.Map<Medida>(request);
        medida.MedicoId = medicoId;

        var creada = await _medidaRepository.CrearAsync(medida);

        await _prediccionRepository.ActualizarPesoRealAsync(
            request.NinoId, request.FechaMedicion, request.Peso);

        _logger.LogInformation(
            "Medida creada para niño ID: {NinoId}, peso: {Peso}kg", request.NinoId, request.Peso);

        var response = _mapper.Map<MedidaResponse>(creada);
        var edadMeses = CalcularEdadMeses(nino.FechaNacimiento, request.FechaMedicion);
        var oms = await _omsRepository.ObtenerPesoPorEdadAsync(nino.Sexo, edadMeses);

        if (oms is not null)
        {
            response.EstadoNutricional = DeterminarEstadoNutricional(request.Peso, oms).ToString();
            response.Percentil = EstimarPercentil(request.Peso, oms);
        }

        return response;
    }

    public async Task<MedidaResponse> ActualizarAsync(int id, ActualizarMedidaRequest request)
    {
        var medida = await _medidaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Medida", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(medida.NinoId)
            ?? throw new NotFoundException("Niño", medida.NinoId);

        medida.FechaMedicion = request.FechaMedicion;
        medida.Peso = request.Peso;
        medida.Talla = request.Talla;

        var actualizada = await _medidaRepository.ActualizarAsync(medida);
        _logger.LogInformation("Medida actualizada con ID: {Id}", id);

        var response = _mapper.Map<MedidaResponse>(actualizada);
        var edadMeses = CalcularEdadMeses(nino.FechaNacimiento, request.FechaMedicion);
        var oms = await _omsRepository.ObtenerPesoPorEdadAsync(nino.Sexo, edadMeses);

        if (oms is not null)
        {
            response.EstadoNutricional = DeterminarEstadoNutricional(request.Peso, oms).ToString();
            response.Percentil = EstimarPercentil(request.Peso, oms);
        }

        return response;
    }

    public async Task EliminarAsync(int id)
    {
        var medida = await _medidaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Medida", id);

        await _medidaRepository.EliminarAsync(medida.Id);
        _logger.LogInformation("Medida eliminada con ID: {Id}", id);
    }

    // ── Métodos privados ──────────────────────────────────────────────────

    private static int CalcularEdadMeses(DateOnly fechaNacimiento, DateOnly fechaMedicion)
    {
        var meses = ((fechaMedicion.Year - fechaNacimiento.Year) * 12)
                  + fechaMedicion.Month - fechaNacimiento.Month;

        if (fechaMedicion.Day < fechaNacimiento.Day)
            meses--;

        return Math.Max(0, meses);
    }

    private static EstadoNutricional DeterminarEstadoNutricional(
        decimal peso, OmsPesoPorEdad oms) => peso switch
        {
            _ when peso < oms.Percentil3 => EstadoNutricional.BajoPesoSevero,
            _ when peso < oms.Percentil15 => EstadoNutricional.BajoPeso,
            _ when peso < oms.Percentil85 => EstadoNutricional.Normal,
            _ when peso < oms.Percentil97 => EstadoNutricional.Sobrepeso,
            _ => EstadoNutricional.Obesidad
        };

    private static int EstimarPercentil(decimal peso, OmsPesoPorEdad oms)
    {
        if (peso <= oms.Percentil3) return 3;
        if (peso <= oms.Percentil15) return Interpolar(peso, oms.Percentil3, oms.Percentil15, 3, 15);
        if (peso <= oms.Percentil50) return Interpolar(peso, oms.Percentil15, oms.Percentil50, 15, 50);
        if (peso <= oms.Percentil85) return Interpolar(peso, oms.Percentil50, oms.Percentil85, 50, 85);
        if (peso <= oms.Percentil97) return Interpolar(peso, oms.Percentil85, oms.Percentil97, 85, 97);
        return 97;
    }

    private static int Interpolar(decimal valor, decimal min, decimal max, int pMin, int pMax)
    {
        if (max == min) return pMin;
        var ratio = (double)(valor - min) / (double)(max - min);
        return (int)Math.Round(pMin + ratio * (pMax - pMin));
    }
}
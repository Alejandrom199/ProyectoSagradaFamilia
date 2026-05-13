using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Exceptions;

namespace SagradaFamilia.Application.Services;

public class PrediccionService : IPrediccionService
{
    private readonly INinoRepository _ninoRepository;
    private readonly IMedidaRepository _medidaRepository;
    private readonly IProphetApiClient _prophetClient;

    public PrediccionService(
        INinoRepository ninoRepository,
        IMedidaRepository medidaRepository,
        IProphetApiClient prophetClient)
    {
        _ninoRepository = ninoRepository;
        _medidaRepository = medidaRepository;
        _prophetClient = prophetClient;
    }

    public async Task<PrediccionDto.Response> ObtenerPrediccionesAsync(int ninoId)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
                   ?? throw new NotFoundException("Niño", ninoId);

        // 1. Usamos el nombre exacto de tu interfaz: ObtenerPorNinoAsync
        var medidas = await _medidaRepository.ObtenerPorNinoAsync(ninoId);

        if (medidas.Count() < 3)
        {
            return new PrediccionDto.Response
            {
                PuedePredecir = false,
                Mensaje = "Se requieren al menos 3 medidas históricas para generar una predicción confiable.",
                NinoId = ninoId
            };
        }

        var historico = medidas
            .Select(m => (m.FechaMedicion, m.Peso))
            .ToList();

        var ultimaMedida = medidas.OrderByDescending(m => m.FechaMedicion).First();

        int edadMeses = ((DateTime.Now.Year - nino.FechaNacimiento.Year) * 12) +
                         DateTime.Now.Month - nino.FechaNacimiento.Month;

        return await _prophetClient.PredecirPesoAsync(
            edadMeses,
            nino.Sexo,
            historico,
            ultimaMedida.Peso,
            ultimaMedida.Talla
        );
    }

    public async Task<PrediccionDto.Health> ObtenerEstadoServicioPrediccionAsync()
    {
        return await _prophetClient.EstadoServicioPredecirAsync();
    }
}
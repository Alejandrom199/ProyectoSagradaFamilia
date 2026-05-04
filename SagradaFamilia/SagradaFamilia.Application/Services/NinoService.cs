namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Ninos;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class NinoService : INinoService
{
    private readonly INinoRepository _ninoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<NinoService> _logger;

    public NinoService(
        INinoRepository ninoRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<NinoService> logger)
    {
        _ninoRepository = ninoRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<NinoResponse> ObtenerPorIdAsync(int id)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        return _mapper.Map<NinoResponse>(nino);
    }

    public async Task<IEnumerable<NinoResponse>> ObtenerTodosAsync()
    {
        var ninos = await _ninoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<NinoResponse>>(ninos);
    }

    public async Task<IEnumerable<NinoResponse>> ObtenerPorRepresentanteAsync(int representanteId)
    {
        var ninos = await _ninoRepository.ObtenerPorRepresentanteAsync(representanteId);
        return _mapper.Map<IEnumerable<NinoResponse>>(ninos);
    }

    public async Task<NinoResponse> CrearAsync(CrearNinoRequest request)
    {
        var representante = await _usuarioRepository.ObtenerPorIdAsync(request.RepresentanteId)
            ?? throw new NotFoundException("Representante", request.RepresentanteId);

        // Verifica que el representante sea un padre usando el enum
        if (representante.RolId != (int)RolEnum.Padre)
            throw new BusinessException("El representante debe ser un padre de familia.");

        var nino = _mapper.Map<Nino>(request);
        var creado = await _ninoRepository.CrearAsync(nino);

        _logger.LogInformation("Niño creado con ID: {Id}", creado.Id);

        var ninoConPadre = await _ninoRepository.ObtenerPorIdAsync(creado.Id)
            ?? throw new NotFoundException("Niño", creado.Id);

        return _mapper.Map<NinoResponse>(ninoConPadre);
    }

    public async Task<NinoResponse> ActualizarAsync(int id, ActualizarNinoRequest request)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        nino.Nombre = request.Nombre;
        nino.Apellido = request.Apellido;
        nino.FechaNacimiento = request.FechaNacimiento;
        nino.Sexo = request.Sexo;

        var actualizado = await _ninoRepository.ActualizarAsync(nino);
        _logger.LogInformation("Niño actualizado con ID: {Id}", id);

        return _mapper.Map<NinoResponse>(actualizado);
    }

    public async Task EliminarAsync(int id)
    {
        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        await _ninoRepository.EliminarAsync(nino.Id);
        _logger.LogInformation("Niño eliminado con ID: {Id}", id);
    }
}
namespace SagradaFamilia.Application.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class PadreService : IPadreService
{
    private readonly IPadreRepository _padreRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PadreService> _logger;

    public PadreService(
        IPadreRepository padreRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PadreService> logger)
    {
        _padreRepository = padreRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PadreDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando el listado general de representantes (Padres).");

        var padres = await _padreRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<PadreDto.ListResponse>>(padres);
    }

    public async Task<PadreDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando detalle del padre con ID: {Id}", id);

        var padre = await _padreRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Padre", id);

        return _mapper.Map<PadreDto.DetailResponse>(padre);
    }

    public async Task<PadreDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("Buscando perfil de padre para el Usuario ID: {UsuarioId}", usuarioId);

        var padre = await _padreRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Padre asociado al usuario", usuarioId);

        return _mapper.Map<PadreDto.DetailResponse>(padre);
    }

    public async Task<PadreDto.DetailResponse> CrearAsync(PadreDto.Create request)
    {
        _logger.LogInformation("Iniciando proceso de registro para el padre: {Nombre} {Apellido}", request.Nombre, request.Apellido);

        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
        {
            _logger.LogWarning("Intento de registro fallido: El email {Email} ya está en uso.", request.Email);
            throw new BusinessException("El correo electrónico ya se encuentra registrado.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var usuario = new Usuario
            {
                Email = request.Email,
                PasswordHash = BCrypt.HashPassword(request.Password),
                RolId = (int)RolEnum.Padre,
                Activo = true
            };
            await _usuarioRepository.CrearAsync(usuario);
            _logger.LogDebug("Cuenta de usuario creada para {Email} con ID: {UId}", request.Email, usuario.Id);

            var padre = new Padre
            {
                UsuarioId = usuario.Id,
                MedicoId = request.MedicoId,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Telefono = request.Telefono
            };
            var creado = await _padreRepository.CrearAsync(padre);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Padre '{Nombre}' registrado exitosamente con ID: {Id}", creado.Nombre, creado.Id);

            var padreCompleto = await _padreRepository.ObtenerPorIdAsync(creado.Id);
            return _mapper.Map<PadreDto.DetailResponse>(padreCompleto!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error crítico durante la creación del padre. Se realizó Rollback.");
            throw new BusinessException("No se pudo completar el registro del representante.");
        }
    }

    public async Task<PadreDto.DetailResponse> ActualizarAsync(int id, PadreDto.Update request)
    {
        _logger.LogInformation("Actualizando datos del padre ID: {Id}", id);

        var padre = await _padreRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Padre", id);

        _mapper.Map(request, padre);

        if (request.MedicoId.HasValue)
        {
            padre.MedicoId = request.MedicoId.Value;
        }

        var actualizado = await _padreRepository.ActualizarAsync(padre);

        _logger.LogInformation("Padre ID: {Id} actualizado correctamente.", id);

        var padreCompleto = await _padreRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<PadreDto.DetailResponse>(padreCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Iniciando eliminación del padre ID: {Id}", id);

        var padre = await _padreRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Padre", id);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _padreRepository.EliminarAsync(padre.Id);
            await _usuarioRepository.EliminarAsync(padre.UsuarioId);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Padre ID: {Id} y su cuenta de usuario asociada han sido eliminados.", id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar el padre ID: {Id}. Se realizó Rollback.", id);
            throw new BusinessException("No se pudo eliminar el registro del representante.");
        }
    }
}
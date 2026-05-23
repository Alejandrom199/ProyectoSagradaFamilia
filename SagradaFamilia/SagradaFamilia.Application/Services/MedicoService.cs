namespace SagradaFamilia.Application.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicoService> _logger;

    public MedicoService(
        IMedicoRepository medicoRepository,
        IUsuarioRepository usuarioRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> appSettings,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MedicoService> logger)
    {
        _medicoRepository = medicoRepository;
        _usuarioRepository = usuarioRepository;
        _resetTokenRepository = resetTokenRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _appSettings = appSettings.Value;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicoDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando el listado general de médicos registrados.");

        var medicos = await _medicoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<MedicoDto.ListResponse>>(medicos);
    }

    public async Task<MedicoDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando detalle del médico con ID: {Id}", id);

        var medico = await _medicoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Médico", id);

        return _mapper.Map<MedicoDto.DetailResponse>(medico);
    }

    public async Task<MedicoDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("Recuperando perfil médico para el Usuario ID: {UsuarioId}", usuarioId);

        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Médico asociado al usuario", usuarioId);

        return _mapper.Map<MedicoDto.DetailResponse>(medico);
    }

    public async Task<MedicoDto.DetailResponse> ObtenerConPacientesAsync(int id)
    {
        _logger.LogInformation("Obteniendo médico ID: {Id} incluyendo su lista de pacientes asignados.", id);

        var medico = await _medicoRepository.ObtenerConPacientesAsync(id)
            ?? throw new NotFoundException("Médico", id);

        return _mapper.Map<MedicoDto.DetailResponse>(medico);
    }

    public async Task<MedicoDto.DetailResponse> CrearAsync(MedicoDto.Create request)
    {
        _logger.LogInformation("Iniciando registro de nuevo médico: {Nombre} {Apellido} con Email: {Email}",
            request.Nombre, request.Apellido, request.Email);

        // 1. Validación de duplicidad de correo electrónico
        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
        {
            _logger.LogWarning("Fallo en registro: El email {Email} ya se encuentra en uso.", request.Email);
            throw new BusinessException("Ya existe un médico registrado con ese correo electrónico.");
        }

        // 2. Transacción para asegurar la creación atómica de Usuario + Medico
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Crear cuenta de acceso al sistema
            var usuario = new Usuario
            {
                Email = request.Email,
                PasswordHash = BCrypt.HashPassword(request.Password),
                RolId = (int)RolEnum.Medico, // 2 para Médico
                Activo = true
            };
            await _usuarioRepository.CrearAsync(usuario);
            _logger.LogDebug("Cuenta de usuario creada con ID: {UId}", usuario.Id);

            // Crear perfil profesional vinculado al usuario
            var medico = new Medico
            {
                UsuarioId = usuario.Id,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Especialidad = request.Especialidad,
                Telefono = request.Telefono
            };
            var creado = await _medicoRepository.CrearAsync(medico);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Médico '{Nombre} {Apellido}' registrado exitosamente con ID: {Id}",
                creado.Nombre, creado.Apellido, creado.Id);

            // Devolver detalle completo
            var medicoCompleto = await _medicoRepository.ObtenerPorIdAsync(creado.Id);
            return _mapper.Map<MedicoDto.DetailResponse>(medicoCompleto!);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error crítico durante la creación del médico. Se revirtieron los cambios (Rollback).");
            throw new BusinessException("No se pudo completar el registro del médico.");
        }
    }

    public async Task<MedicoDto.DetailResponse> ActualizarAsync(int id, MedicoDto.Update request)
    {
        _logger.LogInformation("Actualizando información profesional del médico ID: {Id}", id);

        var medico = await _medicoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Médico", id);

        // Mapeo de actualización (Nombre, Apellido, Especialidad, Telefono)
        _mapper.Map(request, medico);

        var actualizado = await _medicoRepository.ActualizarAsync(medico);

        _logger.LogInformation("Médico ID: {Id} actualizado correctamente.", id);

        var medicoCompleto = await _medicoRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<MedicoDto.DetailResponse>(medicoCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Iniciando proceso de eliminación para el médico ID: {Id}", id);

        var medico = await _medicoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Médico", id);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _medicoRepository.EliminarAsync(medico.Id);
            await _usuarioRepository.EliminarAsync(medico.UsuarioId);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Médico ID: {Id} y su cuenta de usuario asociada han sido eliminados lógicamente.", id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar el médico ID: {Id}. Se realizó Rollback.", id);
            throw new BusinessException("No se pudo eliminar el registro del médico.");
        }
    }

    public async Task RestablecerPasswordAsync(int medicoId)
    {
        _logger.LogInformation("Solicitud de restablecimiento de contraseña para médico ID: {Id}", medicoId);

        var medico = await _medicoRepository.ObtenerPorIdAsync(medicoId)
            ?? throw new NotFoundException("Médico", medicoId);

        await _resetTokenRepository.InvalidarTokensAnterioresAsync(medico.UsuarioId);

        var token = new PasswordResetToken
        {
            UsuarioId = medico.UsuarioId,
            Token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            FechaExpiracion = DateTime.UtcNow.AddHours(24)
        };
        await _resetTokenRepository.CrearAsync(token);

        var link = $"{_appSettings.FrontendUrl}/nueva-clave?token={token.Token}";
        var nombre = $"{medico.Nombre} {medico.Apellido}";
        var cuerpo = _emailTemplateService.GenerarResetPassword(nombre, link);

        await _emailService.EnviarAsync(medico.Usuario.Email, "Restablecimiento de contraseña", cuerpo);
        _logger.LogInformation("Email de restablecimiento enviado a {Email} para médico ID: {Id}", medico.Usuario.Email, medicoId);
    }
}
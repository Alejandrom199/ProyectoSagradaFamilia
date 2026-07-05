namespace SagradaFamilia.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Reporting.Excel.Documents;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IMapper _mapper;
    private readonly ILogger<CitaService> _logger;

    public CitaService(
        ICitaRepository citaRepository,
        IMedicoRepository medicoRepository,
        IParametroRepository parametroRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IMapper mapper,
        ILogger<CitaService> logger)
    {
        _citaRepository = citaRepository;
        _medicoRepository = medicoRepository;
        _parametroRepository = parametroRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CitaDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando información de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        return _mapper.Map<CitaDto.Response>(cita);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerHistorialPorMedicoAsync(int usuarioId)
    {
        _logger.LogInformation("Consultando historial completo del médico ID: {UsuarioId}", usuarioId);
        var citas = await _citaRepository.ObtenerHistorialPorMedicoAsync(usuarioId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorNinoIdAsync(int ninoId)
    {
        _logger.LogInformation("Consultando historial de citas para el niño ID: {NinoId}", ninoId);

        var citas = await _citaRepository.ObtenerPorNinoIdAsync(ninoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<byte[]> ExportarExcelPorNinoAsync(int ninoId)
    {
        var citas = await ObtenerPorNinoIdAsync(ninoId);
        return new CitasExportDocument(citas).GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelPorMedicoAsync(int usuarioId)
    {
        var citas = await ObtenerHistorialPorMedicoAsync(usuarioId);
        return new CitasExportDocument(citas).GenerarBytes();
    }

    public async Task<(IEnumerable<CitaDto.Response> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(
        int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _citaRepository.ObtenerPaginadoPorNinoAsync(ninoId, page, pageSize, search, sortBy, ascending);
        return (_mapper.Map<IEnumerable<CitaDto.Response>>(items), total);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorMedicoIdAsync(int medicoId, DateOnly fecha)
    {
        _logger.LogInformation("Consultando agenda del médico ID: {MedicoId} para la fecha: {Fecha}", medicoId, fecha);

        var citas = await _citaRepository.ObtenerPorMedicoIdAsync(medicoId, fecha);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPendientesPorMedicoAsync(int medicoId)
    {
        _logger.LogInformation("Obteniendo todas las citas pendientes para el médico ID: {MedicoId}", medicoId);

        var citas = await _citaRepository.ObtenerPendientesPorMedicoAsync(medicoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<CitaDto.Response> CrearAsync(CitaDto.Create request, int usuarioId)
    {
        _logger.LogInformation("Intentando agendar nueva cita para el Niño ID: {NinoId} con el Médico UsuarioId: {UsuarioId}",
            request.NinoId, usuarioId);

        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Médico", usuarioId);

        await ValidarHorarioAtencionAsync(request.FechaHora);

        if (request.FechaHoraFin <= request.FechaHora)
            throw new BusinessException("La hora de terminación debe ser posterior a la hora de inicio.");

        var haySolapamiento = await _citaRepository.ExisteTraslapeAsync(
            medico.Id, request.FechaHora, request.FechaHoraFin);

        if (haySolapamiento)
            throw new BusinessException(
                $"El médico ya tiene una cita programada que se solapa con el horario " +
                $"{request.FechaHora:HH:mm}–{request.FechaHoraFin:HH:mm}. " +
                $"Verifique la agenda antes de agendar.");

        var cita = _mapper.Map<Cita>(request);
        cita.MedicoId = medico.Id;
        cita.Estado = EstadoCita.Pendiente;
        cita.UsuarioCreacionId = usuarioId;

        var creada = await _citaRepository.CrearAsync(cita);

        _logger.LogInformation("Cita agendada exitosamente con ID: {Id} para la fecha/hora: {FechaHora}",
            creada.Id, creada.FechaHora);

        var citaCompleta = await _citaRepository.ObtenerPorIdAsync(creada.Id);

        // Se espera (no fire-and-forget): el DbContext es scoped y se destruye al terminar el request,
        // así que un envío en segundo plano fallaría al consultar la plantilla de correo.
        await EnviarCorreoCitaAsync("CITA_AGENDADA", citaCompleta!);

        return _mapper.Map<CitaDto.Response>(citaCompleta!);
    }

    public async Task<CitaDto.Response> ActualizarAsync(int id, CitaDto.Update request, int usuarioId)
    {
        _logger.LogInformation("Iniciando reagendación de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        var estadosReagendables = new[] { EstadoCita.Pendiente, EstadoCita.Cancelada, EstadoCita.NoAsistio };
        if (!estadosReagendables.Contains(cita.Estado))
            throw new BusinessException("Solo se pueden reagendar citas pendientes, canceladas o con ausencia.");

        if (request.FechaHoraFin <= request.FechaHora)
            throw new BusinessException("La hora de terminación debe ser posterior a la hora de inicio.");

        var haySolapamiento = await _citaRepository.ExisteTraslapeAsync(
            cita.MedicoId, request.FechaHora, request.FechaHoraFin, excluirCitaId: id);

        if (haySolapamiento)
            throw new BusinessException(
                $"El médico ya tiene una cita que se solapa con el horario " +
                $"{request.FechaHora:HH:mm}–{request.FechaHoraFin:HH:mm}.");

        _mapper.Map(request, cita);
        cita.UsuarioModificacionId = usuarioId;

        // Al reagendar una cita cancelada o con ausencia, vuelve a Pendiente
        if (cita.Estado == EstadoCita.Cancelada || cita.Estado == EstadoCita.NoAsistio)
            cita.Estado = EstadoCita.Pendiente;

        var actualizada = await _citaRepository.ActualizarAsync(cita);

        _logger.LogInformation("Cita ID: {Id} reagendada correctamente. Nueva Fecha/Hora: {FechaHora}",
            id, actualizada.FechaHora);

        var citaCompleta = await _citaRepository.ObtenerPorIdAsync(actualizada.Id);

        await EnviarCorreoCitaAsync("CITA_REAGENDADA", citaCompleta!);

        return _mapper.Map<CitaDto.Response>(citaCompleta!);
    }

    public async Task ActualizarEstadoAsync(int id, CitaDto.CambiarEstadoRequest request)
    {
        _logger.LogInformation("Cambiando estado de la cita ID: {Id} a {Estado}", id, request.Estado);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        if (request.Estado != EstadoCita.Cancelada && DateTime.Now < cita.FechaHora)
            throw new BusinessException("No es posible gestionar esta cita antes de su fecha y hora programada.");

        var estadoAnterior = cita.Estado;
        cita.Estado = request.Estado;

        if ((request.Estado == EstadoCita.Cancelada || request.Estado == EstadoCita.NoAsistio)
            && !string.IsNullOrWhiteSpace(request.MotivoCancelacion))
        {
            cita.MotivoCancelacion = request.MotivoCancelacion.Trim();
        }

        await _citaRepository.ActualizarAsync(cita);

        _logger.LogInformation("Estado de la cita ID: {Id} cambiado de {Anterior} a {Nuevo}",
            id, estadoAnterior, request.Estado);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Se ha solicitado la eliminación de la cita ID: {Id}", id);

        var cita = await _citaRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Cita", id);

        await _citaRepository.EliminarAsync(cita.Id);

        _logger.LogInformation("Cita ID: {Id} eliminada correctamente del sistema.", id);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerPorPadreIdAsync(int padreId)
    {
        _logger.LogInformation("Consultando citas de los hijos del padre ID: {PadreId}", padreId);

        var citas = await _citaRepository.ObtenerPorPadreIdAsync(padreId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    public async Task<IEnumerable<CitaDto.Response>> ObtenerProximasPorMedicoAsync(int medicoId)
    {
        _logger.LogInformation("Consultando agenda futura del médico ID: {MedicoId}", medicoId);
        var citas = await _citaRepository.ObtenerPendientesPorMedicoAsync(medicoId);
        return _mapper.Map<IEnumerable<CitaDto.Response>>(citas);
    }

    private async Task EnviarCorreoCitaAsync(string codigoEvento, Cita cita)
    {
        try
        {
            var emailPadre = cita.Nino?.Padre?.Usuario?.Email;
            if (string.IsNullOrWhiteSpace(emailPadre)) return;

            var variables = new Dictionary<string, string>
            {
                ["NOMBRE_PADRE"] = cita.Nino.Padre.Nombre,
                ["NOMBRE_NINO"]  = $"{cita.Nino.Nombre} {cita.Nino.Apellido}",
                ["FECHA"]        = cita.FechaHora.ToString("dd/MM/yyyy"),
                ["HORA_INICIO"]  = cita.FechaHora.ToString("HH:mm"),
                ["HORA_FIN"]     = cita.FechaHoraFin?.ToString("HH:mm") ?? "—",
                ["MEDICO"]       = $"Dr(a). {cita.Medico.Nombre} {cita.Medico.Apellido}",
                ["MOTIVO"]       = !string.IsNullOrWhiteSpace(cita.Motivo) ? cita.Motivo : "No especificado",
            };

            var (asunto, cuerpo) = await _emailTemplateService.GenerarAsync(codigoEvento, variables);

            var incluyeInvitacion = codigoEvento is "CITA_AGENDADA" or "CITA_REAGENDADA";
            (string Nombre, byte[] Contenido)? adjunto = incluyeInvitacion ? GenerarInvitacionIcs(cita) : null;

            await _emailService.EnviarAsync(emailPadre, asunto, cuerpo, adjunto);

            _logger.LogInformation("Correo {Evento} enviado a {Email} para cita ID: {CitaId}", codigoEvento, emailPadre, cita.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo enviar el correo {Evento} para cita ID: {CitaId}", codigoEvento, cita.Id);
        }
    }

    // Ecuador (America/Guayaquil) es UTC-5 fijo, sin horario de verano.
    private const int OffsetHorasEcuador = 5;

    private static (string Nombre, byte[] Contenido) GenerarInvitacionIcs(Cita cita)
    {
        var inicioUtc = cita.FechaHora.AddHours(OffsetHorasEcuador);
        var finUtc = (cita.FechaHoraFin ?? cita.FechaHora.AddMinutes(30)).AddHours(OffsetHorasEcuador);

        static string FormatoUtc(DateTime dt) => dt.ToString("yyyyMMddTHHmmssZ");
        static string Escapar(string texto) => texto
            .Replace("\\", "\\\\").Replace(",", "\\,").Replace(";", "\\;")
            .Replace("\r\n", "\\n").Replace("\n", "\\n");

        var descripcion = $"Cita con Dr(a). {cita.Medico.Nombre} {cita.Medico.Apellido}"
            + (!string.IsNullOrWhiteSpace(cita.Motivo) ? $" — {cita.Motivo}" : "");

        var ics = new System.Text.StringBuilder()
            .Append("BEGIN:VCALENDAR\r\n")
            .Append("VERSION:2.0\r\n")
            .Append("PRODID:-//Sagrada Familia//Citas//ES\r\n")
            .Append("CALSCALE:GREGORIAN\r\n")
            .Append("METHOD:PUBLISH\r\n")
            .Append("BEGIN:VEVENT\r\n")
            .Append($"UID:cita-{cita.Id}@sagradafamilia\r\n")
            .Append($"DTSTAMP:{FormatoUtc(DateTime.UtcNow)}\r\n")
            .Append($"DTSTART:{FormatoUtc(inicioUtc)}\r\n")
            .Append($"DTEND:{FormatoUtc(finUtc)}\r\n")
            .Append($"SUMMARY:{Escapar($"Cita médica de {cita.Nino.Nombre} {cita.Nino.Apellido}")}\r\n")
            .Append($"DESCRIPTION:{Escapar(descripcion)}\r\n")
            .Append("STATUS:CONFIRMED\r\n")
            .Append("SEQUENCE:0\r\n")
            .Append("END:VEVENT\r\n")
            .Append("END:VCALENDAR\r\n")
            .ToString();

        return ("cita.ics", System.Text.Encoding.UTF8.GetBytes(ics));
    }

    private async Task ValidarHorarioAtencionAsync(DateTime fechaHora)
    {
        var pHoraInicio = await _parametroRepository.ObtenerPorGrupoYCodigoAsync("HORARIO_ATENCION", "HORA_INICIO");
        var pHoraFin = await _parametroRepository.ObtenerPorGrupoYCodigoAsync("HORARIO_ATENCION", "HORA_FIN");
        var pDiasHabiles = await _parametroRepository.ObtenerPorGrupoYCodigoAsync("HORARIO_ATENCION", "DIAS_HABILES");

        if (pHoraInicio is not null && TimeOnly.TryParse(pHoraInicio.Valor, out var horaInicio))
        {
            if (TimeOnly.FromDateTime(fechaHora) < horaInicio)
                throw new BusinessException($"Las citas no pueden agendarse antes de las {pHoraInicio.Valor} horas.");
        }

        if (pHoraFin is not null && TimeOnly.TryParse(pHoraFin.Valor, out var horaFin))
        {
            if (TimeOnly.FromDateTime(fechaHora) >= horaFin)
                throw new BusinessException($"Las citas no pueden agendarse después de las {pHoraFin.Valor} horas.");
        }

        if (pDiasHabiles is not null)
        {
            var diasHabiles = pDiasHabiles.Valor
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => int.TryParse(d.Trim(), out var n) ? (int?)n : null)
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .ToList();

            // DayOfWeek: Sunday=0, Monday=1 ... pero el parámetro usa 1=Lunes, 7=Domingo
            int diaSemana = fechaHora.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fechaHora.DayOfWeek;

            if (diasHabiles.Count > 0 && !diasHabiles.Contains(diaSemana))
                throw new BusinessException("Las citas solo pueden agendarse en días hábiles de atención.");
        }
    }

}
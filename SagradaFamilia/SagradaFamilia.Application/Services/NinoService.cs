namespace SagradaFamilia.Application.Services;

using AutoMapper;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Reporting.Excel.Documents;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class NinoService : INinoService
{
    private readonly INinoRepository _ninoRepository;
    private readonly IPadreRepository _padreRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICitaRepository _citaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<NinoService> _logger;

    public NinoService(
        INinoRepository ninoRepository,
        IPadreRepository padreRepository,
        IMedicoRepository medicoRepository,
        IUsuarioRepository usuarioRepository,
        ICitaRepository citaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<NinoService> logger)
    {
        _ninoRepository = ninoRepository;
        _padreRepository = padreRepository;
        _medicoRepository = medicoRepository;
        _usuarioRepository = usuarioRepository;
        _citaRepository = citaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }


    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando el listado global de niños en el sistema.");

        var ninos = await _ninoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }

    public async Task<PagedResponse<NinoDto.ListResponse>> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending, int? medicoId = null)
    {
        var (items, total) = await _ninoRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending, medicoId);
        var data = _mapper.Map<IEnumerable<NinoDto.ListResponse>>(items);
        return PagedResponse<NinoDto.ListResponse>.Ok(data, total, page, pageSize);
    }

    public async Task<NinoDto.DetailResponse> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando información detallada del niño con ID: {Id}", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        return _mapper.Map<NinoDto.DetailResponse>(nino);
    }

    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorPadreIdAsync(int padreId)
    {
        _logger.LogInformation("Obteniendo lista de hijos para el Padre ID: {PadreId}", padreId);

        var ninos = await _ninoRepository.ObtenerPorPadreIdAsync(padreId);
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }

    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorMedicoIdAsync(int medicoId)
    {
        _logger.LogInformation("Obteniendo lista de pacientes para el Médico ID: {MedicoId}", medicoId);

        var ninos = await _ninoRepository.ObtenerPorMedicoIdAsync(medicoId);
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }


    public async Task<bool> PerteneceAPadreAsync(int ninoId, int padreId)
    {
        _logger.LogDebug("Verificando si el niño {NinoId} pertenece al padre {PadreId}", ninoId, padreId);
        return await _ninoRepository.PerteneceAPadreAsync(ninoId, padreId);
    }

    public async Task<bool> PerteneceAMedicoAsync(int ninoId, int medicoId)
    {
        _logger.LogDebug("Verificando si el niño {NinoId} es paciente del médico {MedicoId}", ninoId, medicoId);
        return await _ninoRepository.PerteneceAMedicoAsync(ninoId, medicoId);
    }


    public async Task<NinoDto.DetailResponse> CrearAsync(NinoDto.Create request)
    {
        _logger.LogInformation("Registrando nuevo niño: {Nombre} {Apellido}", request.Nombre, request.Apellido);

        var nino = _mapper.Map<Nino>(request);

        var medico = await _medicoRepository.ObtenerPorIdAsync(request.MedicoId)
            ?? throw new NotFoundException("Médico", request.MedicoId);

        nino.MedicoId = medico.Id;

        var creado = await _ninoRepository.CrearAsync(nino);

        _logger.LogInformation("Niño registrado exitosamente con ID: {Id} y asignado al Médico ID: {MedicoId}",
            creado.Id, request.MedicoId);

        var ninoCompleto = await _ninoRepository.ObtenerPorIdAsync(creado.Id);
        return _mapper.Map<NinoDto.DetailResponse>(ninoCompleto!);
    }

    public async Task<NinoDto.DetailResponse> ActualizarAsync(int id, NinoDto.Update request)
    {
        _logger.LogInformation("Iniciando actualización de datos para el niño ID: {Id}", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        _mapper.Map(request, nino);

        var actualizado = await _ninoRepository.ActualizarAsync(nino);

        _logger.LogInformation("Datos del niño ID: {Id} actualizados correctamente.", id);

        var ninoCompleto = await _ninoRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<NinoDto.DetailResponse>(ninoCompleto!);
    }

    public async Task CambiarMedicoAsync(int ninoId, int nuevoMedicoId)
    {
        _logger.LogInformation("Solicitud de reasignación de médico para niño ID: {Id} -> Médico ID: {MedicoId}", ninoId, nuevoMedicoId);

        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
            ?? throw new NotFoundException("Niño", ninoId);

        var medico = await _medicoRepository.ObtenerPorIdAsync(nuevoMedicoId)
            ?? throw new NotFoundException("Médico", nuevoMedicoId);

        nino.MedicoId = medico.Id;
        await _ninoRepository.ActualizarAsync(nino);

        _logger.LogInformation("Niño ID: {Id} reasignado al médico ID: {MedicoId}.", ninoId, nuevoMedicoId);
    }

    public async Task CambiarPadreAsync(int ninoId, int nuevoPadreId)
    {
        _logger.LogInformation("Solicitud de reasignación de representante para niño ID: {Id} -> Padre ID: {PadreId}", ninoId, nuevoPadreId);

        var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId)
            ?? throw new NotFoundException("Niño", ninoId);

        var padre = await _padreRepository.ObtenerPorIdAsync(nuevoPadreId)
            ?? throw new NotFoundException("Padre", nuevoPadreId);

        nino.PadreId = padre.Id;
        await _ninoRepository.ActualizarAsync(nino);

        _logger.LogInformation("Niño ID: {Id} reasignado al padre ID: {PadreId}.", ninoId, nuevoPadreId);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Se ha solicitado la eliminación lógica del niño ID: {Id}", id);

        var nino = await _ninoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Niño", id);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _ninoRepository.EliminarAsync(nino.Id);

            // Las citas pasadas (completadas, canceladas, no asistió) se preservan como historial clínico.
            // Solo se cancelan las pendientes, para que dejen de aparecer en la agenda activa del médico.
            var citas = await _citaRepository.ObtenerPorNinoIdAsync(nino.Id);
            foreach (var cita in citas.Where(c => c.Estado == EstadoCita.Pendiente))
            {
                cita.Estado = EstadoCita.Cancelada;
                cita.MotivoCancelacion = "Paciente dado de baja del sistema.";
                await _citaRepository.ActualizarAsync(cita);
            }

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Niño ID: {Id} eliminado lógicamente del sistema. Citas pendientes canceladas.", id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar el niño ID: {Id}. Se realizó Rollback.", id);
            throw new BusinessException("No se pudo eliminar el registro del niño.");
        }
    }

    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerMisPorUsuarioIdAsync(int usuarioId)
    {
        var padre = await _padreRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Padre", usuarioId);

        var ninos = await _ninoRepository.ObtenerPorPadreIdAsync(padre.Id);
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }

    public async Task<IEnumerable<NinoDto.ListResponse>> ObtenerMisPacientesPorUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("Obteniendo pacientes del médico con UsuarioId: {UsuarioId}", usuarioId);

        var medico = await _medicoRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new NotFoundException("Médico", usuarioId);

        var ninos = await _ninoRepository.ObtenerPorMedicoIdAsync(medico.Id);
        return _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
    }

    public async Task<byte[]> GenerarPlantillaAsync()
    {
        _logger.LogInformation("Generando plantilla Excel para importación masiva de pacientes.");

        var padres = await _padreRepository.ObtenerTodosAsync();
        var representantes = padres
            .OrderBy(p => p.Nombre).ThenBy(p => p.Apellido)
            .Select(p => ($"{p.Nombre} {p.Apellido}", p.Usuario.Email))
            .ToList();

        return new NinosPlantillaDocument(representantes).GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelAsync(int? medicoId = null)
    {
        _logger.LogInformation("Exportando listado de pacientes a Excel. MedicoId filtro: {MedicoId}", medicoId);
        var ninos = medicoId.HasValue
            ? await _ninoRepository.ObtenerPorMedicoIdAsync(medicoId.Value)
            : await _ninoRepository.ObtenerTodosAsync();
        var dtos = _mapper.Map<IEnumerable<NinoDto.ListResponse>>(ninos);
        return new NinosExportDocument(dtos).GenerarBytes();
    }

    public async Task<NinoDto.ImportResultado> ImportarAsync(Stream archivoStream, int medicoId)
    {
        _logger.LogInformation("Iniciando importación masiva de pacientes desde Excel para médico ID: {MedicoId}", medicoId);
        var resultado = new NinoDto.ImportResultado();

        XLWorkbook workbook;
        try { workbook = new XLWorkbook(archivoStream); }
        catch (Exception ex)
        {
            _logger.LogWarning("Archivo Excel inválido: {Error}", ex.Message);
            resultado.Errores.Add(new NinoDto.ImportError { Fila = 0, Mensaje = "El archivo no es un Excel válido (.xlsx)." });
            return resultado;
        }

        using (workbook)
        {
            IXLWorksheet ws;
            try { ws = workbook.Worksheet("Pacientes"); }
            catch
            {
                resultado.Errores.Add(new NinoDto.ImportError { Fila = 0, Mensaje = "No se encontró la hoja 'Pacientes'. Use la plantilla oficial." });
                return resultado;
            }

            var todosLosNinos = await _ninoRepository.ObtenerTodosAsync();
            var ninoMap = todosLosNinos
                .GroupBy(n => $"{n.Nombre.Trim().ToLowerInvariant()}|{n.Apellido.Trim().ToLowerInvariant()}|{n.FechaNacimiento:yyyy-MM-dd}")
                .ToDictionary(g => g.Key, g => g.First());

            const int DataStartRow = 7;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? DataStartRow - 1;

            for (int rowNum = DataStartRow; rowNum <= lastRow; rowNum++)
            {
                var row = ws.Row(rowNum);
                string nombre       = row.Cell(1).GetString().Trim();
                string apellido     = row.Cell(2).GetString().Trim();
                string emailPadre   = ExtraerEmailRepresentante(row.Cell(3).GetString());
                string sexoStr      = row.Cell(4).GetString().Trim().ToUpperInvariant();
                var celdaFecha      = row.Cell(5);

                if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(apellido) && string.IsNullOrEmpty(emailPadre))
                    continue;

                resultado.TotalProcesadas++;
                var erroresFila = new List<string>();

                if (string.IsNullOrEmpty(nombre))        erroresFila.Add("Nombre es obligatorio");
                else if (nombre.Length > 100)            erroresFila.Add("Nombre excede 100 caracteres");

                if (string.IsNullOrEmpty(apellido))      erroresFila.Add("Apellido es obligatorio");
                else if (apellido.Length > 100)          erroresFila.Add("Apellido excede 100 caracteres");

                if (string.IsNullOrEmpty(emailPadre))
                    erroresFila.Add("Email del representante es obligatorio");
                else if (!EsEmailValido(emailPadre))
                    erroresFila.Add("Email del representante no tiene formato válido");

                char sexo = 'M';
                if (string.IsNullOrEmpty(sexoStr))
                    erroresFila.Add("Sexo es obligatorio");
                else
                {
                    sexo = sexoStr switch
                    {
                        "M" or "MASCULINO" => 'M',
                        "F" or "FEMENINO"  => 'F',
                        _ => '\0'
                    };
                    if (sexo == '\0')
                        erroresFila.Add("Sexo debe ser Masculino o Femenino");
                }

                DateOnly fechaNacimiento = default;
                if (celdaFecha.IsEmpty())
                    erroresFila.Add("Fecha de nacimiento es obligatoria");
                else if (celdaFecha.TryGetValue(out DateTime fechaDt))
                    fechaNacimiento = DateOnly.FromDateTime(fechaDt);
                else if (DateOnly.TryParseExact(celdaFecha.GetString().Trim(), ["yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy"], out var fechaParsed))
                    fechaNacimiento = fechaParsed;
                else
                    erroresFila.Add("Fecha de nacimiento no tiene formato válido (use yyyy-MM-dd)");

                if (erroresFila.Count > 0)
                {
                    resultado.Errores.Add(new NinoDto.ImportError { Fila = rowNum, Mensaje = string.Join("; ", erroresFila) });
                    continue;
                }

                var usuarioPadre = await _usuarioRepository.ObtenerPorEmailAsync(emailPadre);
                if (usuarioPadre == null)
                {
                    resultado.Errores.Add(new NinoDto.ImportError { Fila = rowNum, Mensaje = $"No existe un representante registrado con el email '{emailPadre}'." });
                    continue;
                }

                var padre = await _padreRepository.ObtenerPorUsuarioIdAsync(usuarioPadre.Id);
                if (padre == null)
                {
                    resultado.Errores.Add(new NinoDto.ImportError { Fila = rowNum, Mensaje = $"El email '{emailPadre}' existe pero no tiene perfil de representante." });
                    continue;
                }

                string clave = $"{nombre.ToLowerInvariant()}|{apellido.ToLowerInvariant()}|{fechaNacimiento:yyyy-MM-dd}";

                if (ninoMap.TryGetValue(clave, out var ninoExistente))
                {
                    ninoExistente.PadreId = padre.Id;
                    ninoExistente.Sexo    = sexo;
                    await _ninoRepository.ActualizarAsync(ninoExistente);
                    resultado.Actualizados++;
                }
                else
                {
                    var nuevoNino = new Nino
                    {
                        Nombre          = nombre,
                        Apellido        = apellido,
                        FechaNacimiento = fechaNacimiento,
                        Sexo            = sexo,
                        PadreId         = padre.Id,
                        MedicoId        = medicoId
                    };
                    var creado = await _ninoRepository.CrearAsync(nuevoNino);
                    ninoMap[clave] = creado;
                    resultado.Importados++;
                }
            }
        }

        _logger.LogInformation(
            "Importación de pacientes finalizada: {I} creados, {A} actualizados, {E} errores de {T} filas.",
            resultado.Importados, resultado.Actualizados, resultado.Errores.Count, resultado.TotalProcesadas);

        return resultado;
    }

    // El combo de la plantilla escribe "Nombre Apellido - email"; se acepta también
    // un email plano (plantillas viejas o texto pegado a mano fuera del desplegable).
    private static string ExtraerEmailRepresentante(string valorCelda)
    {
        var valor = valorCelda.Trim();
        int idx = valor.LastIndexOf(" - ", StringComparison.Ordinal);
        var email = idx >= 0 ? valor[(idx + 3)..] : valor;
        return email.Trim().ToLowerInvariant();
    }

    private static bool EsEmailValido(string email)
    {
        try { var a = new System.Net.Mail.MailAddress(email); return a.Address == email; }
        catch { return false; }
    }
}
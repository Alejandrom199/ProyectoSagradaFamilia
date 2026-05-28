namespace SagradaFamilia.Application.Services;

using AutoMapper;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Reporting.Excel.Documents;
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

    public async Task<(IEnumerable<MedidaDto.Response> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(
        int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _medidaRepository.ObtenerPaginadoPorNinoAsync(ninoId, page, pageSize, search, sortBy, ascending);
        var dtos = _mapper.Map<IEnumerable<MedidaDto.Response>>(items);
        return (dtos, total);
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

    public async Task<byte[]> GenerarPlantillaAsync()
    {
        _logger.LogInformation("Generando plantilla Excel para importación masiva de medidas.");
        return new MedidasPlantillaDocument().GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelAsync()
    {
        _logger.LogInformation("Exportando listado de medidas a Excel.");
        var medidas = await _medidaRepository.ObtenerTodosAsync();
        var dtos = _mapper.Map<IEnumerable<MedidaDto.Response>>(medidas);
        return new MedidasExportDocument(dtos).GenerarBytes();
    }

    public async Task<MedidaDto.ImportResultado> ImportarAsync(Stream archivoStream, int medicoId)
    {
        _logger.LogInformation("Iniciando importación masiva de medidas desde Excel para médico ID: {MedicoId}", medicoId);
        var resultado = new MedidaDto.ImportResultado();

        XLWorkbook workbook;
        try { workbook = new XLWorkbook(archivoStream); }
        catch (Exception ex)
        {
            _logger.LogWarning("Archivo Excel inválido: {Error}", ex.Message);
            resultado.Errores.Add(new MedidaDto.ImportError { Fila = 0, Mensaje = "El archivo no es un Excel válido (.xlsx)." });
            return resultado;
        }

        using (workbook)
        {
            IXLWorksheet ws;
            try { ws = workbook.Worksheet("Medidas"); }
            catch
            {
                resultado.Errores.Add(new MedidaDto.ImportError { Fila = 0, Mensaje = "No se encontró la hoja 'Medidas'. Use la plantilla oficial." });
                return resultado;
            }

            var todosLosNinos = await _ninoRepository.ObtenerPorMedicoIdAsync(medicoId);
            var ninoMap = todosLosNinos
                .GroupBy(n => $"{n.Nombre.Trim().ToLowerInvariant()}|{n.Apellido.Trim().ToLowerInvariant()}")
                .ToDictionary(g => g.Key, g => g.ToList());

            const int DataStartRow = 7;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? DataStartRow - 1;

            for (int rowNum = DataStartRow; rowNum <= lastRow; rowNum++)
            {
                var row = ws.Row(rowNum);
                string nombre   = row.Cell(1).GetString().Trim();
                string apellido = row.Cell(2).GetString().Trim();
                var celdaFecha  = row.Cell(3);
                var celdaPeso   = row.Cell(4);
                var celdaTalla  = row.Cell(5);

                if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(apellido) && celdaFecha.IsEmpty())
                    continue;

                resultado.TotalProcesadas++;
                var erroresFila = new List<string>();

                if (string.IsNullOrEmpty(nombre))   erroresFila.Add("Nombre del paciente es obligatorio");
                if (string.IsNullOrEmpty(apellido)) erroresFila.Add("Apellido del paciente es obligatorio");

                DateOnly fechaMedicion = default;
                if (celdaFecha.IsEmpty())
                    erroresFila.Add("Fecha de medición es obligatoria");
                else if (celdaFecha.TryGetValue(out DateTime fechaDt))
                    fechaMedicion = DateOnly.FromDateTime(fechaDt);
                else if (DateOnly.TryParseExact(celdaFecha.GetString().Trim(), ["yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy"], out var fechaParsed))
                    fechaMedicion = fechaParsed;
                else
                    erroresFila.Add("Fecha de medición no tiene formato válido (use yyyy-MM-dd)");

                decimal peso = 0;
                if (celdaPeso.IsEmpty())
                    erroresFila.Add("Peso es obligatorio");
                else if (!celdaPeso.TryGetValue(out double pesoDouble) || pesoDouble <= 0 || pesoDouble > 300)
                    erroresFila.Add("Peso debe ser un número positivo en kg (máx 300)");
                else
                    peso = (decimal)pesoDouble;

                decimal talla = 0;
                if (celdaTalla.IsEmpty())
                    erroresFila.Add("Talla es obligatoria");
                else if (!celdaTalla.TryGetValue(out double tallaDouble) || tallaDouble <= 0 || tallaDouble > 250)
                    erroresFila.Add("Talla debe ser un número positivo en cm (máx 250)");
                else
                    talla = (decimal)tallaDouble;

                if (erroresFila.Count > 0)
                {
                    resultado.Errores.Add(new MedidaDto.ImportError { Fila = rowNum, Mensaje = string.Join("; ", erroresFila) });
                    continue;
                }

                string clave = $"{nombre.ToLowerInvariant()}|{apellido.ToLowerInvariant()}";
                if (!ninoMap.TryGetValue(clave, out var candidatos) || candidatos.Count == 0)
                {
                    resultado.Errores.Add(new MedidaDto.ImportError { Fila = rowNum, Mensaje = $"No se encontró el paciente '{nombre} {apellido}' asignado a este médico." });
                    continue;
                }
                if (candidatos.Count > 1)
                {
                    resultado.Errores.Add(new MedidaDto.ImportError { Fila = rowNum, Mensaje = $"Existen {candidatos.Count} pacientes con el nombre '{nombre} {apellido}'. Use el formulario individual para desambiguar." });
                    continue;
                }

                var nino = candidatos[0];
                var medidaExistente = await _medidaRepository.ObtenerPorNinoYMesAsync(nino.Id, fechaMedicion.Month, fechaMedicion.Year);

                if (medidaExistente != null)
                {
                    medidaExistente.FechaMedicion = fechaMedicion;
                    medidaExistente.Peso          = peso;
                    medidaExistente.Talla         = talla;
                    await _medidaRepository.ActualizarAsync(medidaExistente);
                    resultado.Actualizados++;
                }
                else
                {
                    var nuevaMedida = new Medida
                    {
                        NinoId        = nino.Id,
                        MedicoId      = medicoId,
                        FechaMedicion = fechaMedicion,
                        Peso          = peso,
                        Talla         = talla
                    };
                    await _medidaRepository.CrearAsync(nuevaMedida);
                    resultado.Importados++;
                }
            }
        }

        _logger.LogInformation(
            "Importación de medidas finalizada: {I} creadas, {A} actualizadas, {E} errores de {T} filas.",
            resultado.Importados, resultado.Actualizados, resultado.Errores.Count, resultado.TotalProcesadas);

        return resultado;
    }
}

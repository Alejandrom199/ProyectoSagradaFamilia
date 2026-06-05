namespace SagradaFamilia.Application.Services;

using AutoMapper;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Reporting.Excel.Documents;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Exceptions;
using SagradaFamilia.Domain.Interfaces.Repositories;

public class AlimentoService : IAlimentoService
{
    private readonly IAlimentoRepository _alimentoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AlimentoService> _logger;

    public AlimentoService(
        IAlimentoRepository alimentoRepository,
        IMapper mapper,
        ILogger<AlimentoService> logger)
    {
        _alimentoRepository = alimentoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AlimentoDto.Response>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo el catálogo completo de alimentos.");

        var alimentos = await _alimentoRepository.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
    }

    public async Task<PagedResponse<AlimentoDto.Response>> ObtenerPaginadoAsync(
        int page, int pageSize, string? search, string? sortBy, bool ascending)
    {
        var (items, total) = await _alimentoRepository.ObtenerPaginadoAsync(page, pageSize, search, sortBy, ascending);
        var dtos = _mapper.Map<IEnumerable<AlimentoDto.Response>>(items);
        return PagedResponse<AlimentoDto.Response>.Ok(dtos, total, page, pageSize);
    }

    public async Task<AlimentoDto.Response> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando alimento con ID: {Id}", id);

        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        return _mapper.Map<AlimentoDto.Response>(alimento);
    }

    public async Task<IEnumerable<AlimentoDto.Response>> ObtenerPorRangoEdadAsync(int edadMeses)
    {
        _logger.LogInformation("Consultando alimentos recomendados para la edad: {Edad} meses", edadMeses);

        var alimentos = await _alimentoRepository.ObtenerPorRangoEdadAsync(edadMeses); // Asumiendo que el repo mantiene este nombre
        return _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
    }

    public async Task<IEnumerable<AlimentoDto.Response>> ObtenerPorCategoriaAsync(int categoriaId)
    {
        _logger.LogInformation("Obteniendo alimentos pertenecientes a la categoría ID: {CategoriaId}", categoriaId);

        var alimentos = await _alimentoRepository.ObtenerPorCategoriaAsync(categoriaId); // Asumiendo que existe en el repo
        return _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
    }

    public async Task<AlimentoDto.Response> CrearAsync(AlimentoDto.Create request)
    {
        _logger.LogInformation("Iniciando creación de nuevo alimento: {Nombre}", request.Nombre);

        var existente = await _alimentoRepository.ObtenerPorNombreYCategoriaAsync(
            request.Nombre.Trim(), request.CategoriaId);

        if (existente is not null)
        {
            _logger.LogInformation("Alimento '{Nombre}' ya existe (ID: {Id}), actualizando.", existente.Nombre, existente.Id);

            existente.EdadMinimaMeses = request.EdadMinimaMeses;
            existente.Descripcion = request.Descripcion;
            existente.Recomendacion = request.Recomendacion;
            existente.Activo = true;

            var actualizado = await _alimentoRepository.ActualizarAsync(existente);
            var alimentoActualizado = await _alimentoRepository.ObtenerPorIdAsync(actualizado.Id);
            return _mapper.Map<AlimentoDto.Response>(alimentoActualizado!);
        }

        var alimento = _mapper.Map<Alimento>(request);
        alimento.Activo = true;

        var creado = await _alimentoRepository.CrearAsync(alimento);

        _logger.LogInformation("Alimento '{Nombre}' creado exitosamente con ID: {Id}", creado.Nombre, creado.Id);

        var alimentoCompleto = await _alimentoRepository.ObtenerPorIdAsync(creado.Id);
        return _mapper.Map<AlimentoDto.Response>(alimentoCompleto!);
    }

    public async Task<AlimentoDto.Response> ActualizarAsync(int id, AlimentoDto.Update request)
    {
        _logger.LogInformation("Iniciando actualización del alimento ID: {Id}", id);

        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        _mapper.Map(request, alimento);

        var actualizado = await _alimentoRepository.ActualizarAsync(alimento);

        _logger.LogInformation("Alimento ID: {Id} actualizado correctamente.", id);

        var alimentoCompleto = await _alimentoRepository.ObtenerPorIdAsync(actualizado.Id);
        return _mapper.Map<AlimentoDto.Response>(alimentoCompleto!);
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogWarning("Intentando eliminar lógicamente el alimento ID: {Id}", id);

        var alimento = await _alimentoRepository.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException("Alimento", id);

        await _alimentoRepository.EliminarAsync(alimento.Id);

        _logger.LogInformation("Alimento ID: {Id} ha sido eliminado lógicamente del sistema.", id);
    }

    public async Task<IEnumerable<CategoriaDto.Response>> ObtenerCategoriasAsync()
    {
        _logger.LogInformation("Obteniendo lista de todas las categorías de alimentos.");

        var categorias = await _alimentoRepository.ObtenerCategoriasAsync();
        return _mapper.Map<IEnumerable<CategoriaDto.Response>>(categorias);
    }

    public async Task<CategoriaDto.Response> ObtenerCategoriaPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando categoría de alimento con ID: {Id}", id);

        var categoria = await _alimentoRepository.ObtenerCategoriaPorIdAsync(id)
            ?? throw new NotFoundException("Categoría de Alimento", id);

        return _mapper.Map<CategoriaDto.Response>(categoria);
    }

    public async Task<CategoriaDto.Response> CrearCategoriaAsync(CategoriaDto.Create request)
    {
        _logger.LogInformation("Creando nueva categoría: {Nombre}", request.Nombre);

        var categoria = _mapper.Map<CategoriaAlimento>(request);
        var creada = await _alimentoRepository.CrearCategoriaAsync(categoria);

        _logger.LogInformation("Categoría '{Nombre}' creada con ID: {Id}", creada.Nombre, creada.Id);

        return _mapper.Map<CategoriaDto.Response>(creada);
    }

    public async Task<CategoriaDto.Response> ActualizarCategoriaAsync(int id, CategoriaDto.Update request)
    {
        _logger.LogInformation("Actualizando categoría ID: {Id}", id);

        var categoria = await _alimentoRepository.ObtenerCategoriaPorIdAsync(id)
            ?? throw new NotFoundException("Categoría de Alimento", id);

        _mapper.Map(request, categoria);
        var actualizada = await _alimentoRepository.ActualizarCategoriaAsync(categoria);

        _logger.LogInformation("Categoría ID: {Id} actualizada con éxito.", id);

        return _mapper.Map<CategoriaDto.Response>(actualizada);
    }

    public async Task EliminarCategoriaAsync(int id)
    {
        _logger.LogWarning("Intentando eliminar categoría ID: {Id}", id);

        var categoria = await _alimentoRepository.ObtenerCategoriaPorIdAsync(id)
            ?? throw new NotFoundException("Categoría de Alimento", id);

        await _alimentoRepository.EliminarCategoriaAsync(categoria.Id);

        _logger.LogInformation("Categoría ID: {Id} eliminada exitosamente.", id);
    }

    public async Task<byte[]> GenerarPlantillaAsync()
    {
        _logger.LogInformation("Generando plantilla Excel para importación masiva de alimentos.");
        var categorias = await _alimentoRepository.ObtenerCategoriasAsync();
        var nombres = categorias.Where(c => c.Activo).Select(c => c.Nombre).OrderBy(n => n).ToList();
        return new AlimentosPlantillaDocument(nombres).GenerarBytes();
    }

    public async Task<byte[]> ExportarExcelAsync()
    {
        _logger.LogInformation("Exportando catálogo de alimentos a Excel.");
        var alimentos = await _alimentoRepository.ObtenerTodosAsync();
        var dtos = _mapper.Map<IEnumerable<AlimentoDto.Response>>(alimentos);
        return new AlimentosExportDocument(dtos).GenerarBytes();
    }

    public async Task<AlimentoDto.ImportResultado> ImportarAsync(Stream archivoStream)
    {
        _logger.LogInformation("Iniciando importación masiva de alimentos desde Excel.");

        var resultado = new AlimentoDto.ImportResultado();

        // Cargar categorías activas para resolver nombre → ID
        var categorias = await _alimentoRepository.ObtenerCategoriasAsync();
        var categoriaMap = categorias
            .Where(c => c.Activo)
            .ToDictionary(
                c => c.Nombre.Trim().ToLowerInvariant(),
                c => c.Id);

        // Cargar alimentos existentes para detectar duplicados por nombre (upsert)
        var alimentosExistentes = await _alimentoRepository.ObtenerTodosAsync();
        var alimentoMap = alimentosExistentes
            .GroupBy(a => a.Nombre.Trim().ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First());

        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(archivoStream);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("El archivo subido no es un Excel válido: {Error}", ex.Message);
            resultado.Errores.Add(new AlimentoDto.ImportError
            {
                Fila = 0,
                Mensaje = "El archivo no es un Excel válido (.xlsx)."
            });
            return resultado;
        }

        using (workbook)
        {
            IXLWorksheet ws;
            try
            {
                ws = workbook.Worksheet("Alimentos");
            }
            catch
            {
                resultado.Errores.Add(new AlimentoDto.ImportError
                {
                    Fila = 0,
                    Mensaje = "No se encontró la hoja 'Alimentos'. Use la plantilla oficial."
                });
                return resultado;
            }

            const int DataStartRow = 7;
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? DataStartRow - 1;

            for (int rowNum = DataStartRow; rowNum <= lastRow; rowNum++)
            {
                var row = ws.Row(rowNum);

                string nombre = row.Cell(1).GetString().Trim();
                string categoriaNombre = row.Cell(2).GetString().Trim();
                string descripcion = row.Cell(4).GetString().Trim();
                string recomendacion = row.Cell(5).GetString().Trim();
                string activoStr = row.Cell(6).GetString().Trim().ToUpperInvariant();

                // Ignorar filas completamente vacías
                if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(categoriaNombre) && row.Cell(3).IsEmpty())
                    continue;

                resultado.TotalProcesadas++;
                var erroresFila = new List<string>();

                // Validar Nombre
                if (string.IsNullOrEmpty(nombre))
                    erroresFila.Add("Nombre es obligatorio");
                else if (nombre.Length > 200)
                    erroresFila.Add("Nombre excede 200 caracteres");

                // Validar y resolver Categoría
                int categoriaId = 0;
                if (string.IsNullOrEmpty(categoriaNombre))
                    erroresFila.Add("Categoría es obligatoria");
                else if (!categoriaMap.TryGetValue(categoriaNombre.ToLowerInvariant(), out categoriaId))
                    erroresFila.Add($"Categoría '{categoriaNombre}' no existe en el sistema");

                // Validar Edad mínima — TryGetValue lee el valor numérico directo, sin problemas de locale
                int edadMinimaMeses = 0;
                var celdaEdad = row.Cell(3);
                if (celdaEdad.IsEmpty())
                {
                    erroresFila.Add("Edad mínima es obligatoria");
                }
                else
                {
                    bool edadValida = celdaEdad.TryGetValue(out double edadDouble)
                        && edadDouble >= 0
                        && edadDouble <= 240
                        && edadDouble == Math.Floor(edadDouble);

                    if (!edadValida)
                        erroresFila.Add("Edad mínima debe ser un entero entre 0 y 240");
                    else
                        edadMinimaMeses = (int)edadDouble;
                }

                // Validar Activo
                bool activo = true;
                if (!string.IsNullOrEmpty(activoStr))
                {
                    activo = activoStr switch
                    {
                        "ACTIVO" or "A" or "SÍ" or "SI" or "1" or "TRUE" => true,
                        "INACTIVO" or "I" or "NO" or "0" or "FALSE"       => false,
                        _ => true
                    };
                }

                // Validar longitudes opcionales
                if (descripcion.Length > 500)
                    erroresFila.Add("Descripción excede 500 caracteres");
                if (recomendacion.Length > 1000)
                    erroresFila.Add("Recomendación excede 1000 caracteres");

                if (erroresFila.Count > 0)
                {
                    resultado.Errores.Add(new AlimentoDto.ImportError
                    {
                        Fila = rowNum,
                        Mensaje = string.Join("; ", erroresFila)
                    });
                    continue;
                }

                string nombreKey = nombre.ToLowerInvariant();
                if (alimentoMap.TryGetValue(nombreKey, out var existente))
                {
                    existente.CategoriaId = categoriaId;
                    existente.Descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion;
                    existente.EdadMinimaMeses = edadMinimaMeses;
                    existente.Recomendacion = string.IsNullOrEmpty(recomendacion) ? null : recomendacion;
                    existente.Activo = activo;
                    await _alimentoRepository.ActualizarAsync(existente);
                    resultado.Actualizados++;
                }
                else
                {
                    var nuevo = new Alimento
                    {
                        CategoriaId = categoriaId,
                        Nombre = nombre,
                        Descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion,
                        EdadMinimaMeses = edadMinimaMeses,
                        Recomendacion = string.IsNullOrEmpty(recomendacion) ? null : recomendacion,
                        Activo = activo
                    };
                    await _alimentoRepository.CrearAsync(nuevo);
                    resultado.Importados++;
                }
            }
        }

        _logger.LogInformation(
            "Importación finalizada: {Importados} creados, {Actualizados} actualizados, {Errores} errores de {Total} filas procesadas.",
            resultado.Importados, resultado.Actualizados, resultado.Errores.Count, resultado.TotalProcesadas);

        return resultado;
    }
}
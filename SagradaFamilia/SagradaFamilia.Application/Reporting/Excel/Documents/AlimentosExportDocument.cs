namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class AlimentosExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<AlimentoDto.Response> _alimentos;

    public AlimentosExportDocument(IEnumerable<AlimentoDto.Response> alimentos)
    {
        _alimentos = alimentos;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Alimentos");

        string[] headers = ["Nombre", "Categoría", "Edad Mínima (meses)", "Descripción", "Recomendación", "Estado"];
        int totalCols = headers.Length;

        // Filas 1-3: encabezado decorativo (logo + título + fecha)
        AgregarEncabezado(ws, "Catálogo de Alimentos", totalCols);

        // Fila 4: encabezados de columna
        EstilarEncabezadoColumnas(ws, 4, headers);

        // Desde fila 5: datos reales
        var lista = _alimentos.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.Nombre;
            ws.Cell(fila, 2).Value = item.CategoriaNombre;
            ws.Cell(fila, 3).Value = item.EdadMinimaMeses;
            ws.Cell(fila, 4).Value = item.Descripcion   ?? string.Empty;
            ws.Cell(fila, 5).Value = item.Recomendacion ?? string.Empty;
            ws.Cell(fila, 6).Value = item.Activo ? "Activo" : "Inactivo";

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 32;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 22;
        ws.Column(4).Width = 42;
        ws.Column(5).Width = 48;
        ws.Column(6).Width = 12;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

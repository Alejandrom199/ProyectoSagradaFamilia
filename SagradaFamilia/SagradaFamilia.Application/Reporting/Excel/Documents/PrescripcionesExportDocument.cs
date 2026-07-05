namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class PrescripcionesExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<PrescripcionDto.Response> _prescripciones;

    public PrescripcionesExportDocument(IEnumerable<PrescripcionDto.Response> prescripciones)
    {
        _prescripciones = prescripciones;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Prescripciones");

        string[] headers = ["Fecha", "Paciente", "Médico", "Medicamentos", "Indicaciones"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Listado de Prescripciones", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _prescripciones.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.FechaCreacion.ToString("dd/MM/yyyy");
            ws.Cell(fila, 2).Value = item.NombreNino;
            ws.Cell(fila, 3).Value = item.NombreMedico;
            ws.Cell(fila, 4).Value = string.Join(", ", item.Medicamentos.Select(m => $"{m.Nombre} ({m.Dosis}, {m.Frecuencia})"));
            ws.Cell(fila, 5).Value = item.Indicaciones ?? string.Empty;

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 26;
        ws.Column(4).Width = 45;
        ws.Column(5).Width = 35;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

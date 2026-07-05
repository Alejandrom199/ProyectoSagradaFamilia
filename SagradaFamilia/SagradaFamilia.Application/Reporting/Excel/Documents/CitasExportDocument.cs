namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class CitasExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<CitaDto.Response> _citas;

    public CitasExportDocument(IEnumerable<CitaDto.Response> citas)
    {
        _citas = citas;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Citas");

        string[] headers = ["Fecha", "Hora", "Paciente", "Médico", "Motivo", "Estado", "Prescripción"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Listado de Citas", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _citas.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.FechaHora.ToString("dd/MM/yyyy");
            ws.Cell(fila, 2).Value = item.FechaHora.ToString("HH:mm");
            ws.Cell(fila, 3).Value = item.NombreNino;
            ws.Cell(fila, 4).Value = item.NombreMedico;
            ws.Cell(fila, 5).Value = item.Motivo ?? string.Empty;
            ws.Cell(fila, 6).Value = item.Estado;
            ws.Cell(fila, 7).Value = item.TienePrescripcion ? "Sí" : "No";

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 10;
        ws.Column(3).Width = 26;
        ws.Column(4).Width = 26;
        ws.Column(5).Width = 30;
        ws.Column(6).Width = 16;
        ws.Column(7).Width = 14;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

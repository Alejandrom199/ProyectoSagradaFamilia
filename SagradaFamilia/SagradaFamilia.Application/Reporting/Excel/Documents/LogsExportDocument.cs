namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class LogsExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<LogSistemaDto.Response> _logs;

    public LogsExportDocument(IEnumerable<LogSistemaDto.Response> logs)
    {
        _logs = logs;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Eventos del Sistema");

        string[] headers = ["Fecha", "Hora", "Nivel", "Mensaje", "Endpoint", "Usuario"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Eventos del Sistema", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _logs.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.FechaHora.ToString("dd/MM/yyyy");
            ws.Cell(fila, 2).Value = item.FechaHora.ToString("HH:mm:ss");
            ws.Cell(fila, 3).Value = item.Nivel;
            ws.Cell(fila, 4).Value = item.Mensaje;
            ws.Cell(fila, 5).Value = item.Endpoint ?? string.Empty;
            ws.Cell(fila, 6).Value = item.UsuarioId?.ToString() ?? "Sistema";

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 12;
        ws.Column(3).Width = 14;
        ws.Column(4).Width = 50;
        ws.Column(5).Width = 30;
        ws.Column(6).Width = 12;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

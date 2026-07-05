namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class AuditoriaExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<AuditoriaDto.Response> _registros;

    public AuditoriaExportDocument(IEnumerable<AuditoriaDto.Response> registros)
    {
        _registros = registros;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Auditoría");

        string[] headers = ["Fecha", "Hora", "Usuario", "Acción", "Tabla", "Registro"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Actividad del Sistema", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _registros.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.Fecha.ToString("dd/MM/yyyy");
            ws.Cell(fila, 2).Value = item.Fecha.ToString("HH:mm:ss");
            ws.Cell(fila, 3).Value = item.UsuarioNombreCompleto ?? item.UsuarioEmail;
            ws.Cell(fila, 4).Value = item.Accion;
            ws.Cell(fila, 5).Value = item.Tabla;
            ws.Cell(fila, 6).Value = item.ClavePrimaria;

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 12;
        ws.Column(3).Width = 30;
        ws.Column(4).Width = 16;
        ws.Column(5).Width = 18;
        ws.Column(6).Width = 14;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

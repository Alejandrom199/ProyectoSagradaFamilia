namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class PadresExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<PadreDto.ListResponse> _padres;

    public PadresExportDocument(IEnumerable<PadreDto.ListResponse> padres)
    {
        _padres = padres;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Representantes");

        string[] headers = ["Nombre", "Apellido", "Email", "Teléfono", "Total Hijos", "Fecha Registro"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Listado de Representantes", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _padres.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.Nombre;
            ws.Cell(fila, 2).Value = item.Apellido;
            ws.Cell(fila, 3).Value = item.Email;
            ws.Cell(fila, 4).Value = item.Telefono ?? string.Empty;
            ws.Cell(fila, 5).Value = item.TotalHijos;
            ws.Cell(fila, 6).Value = item.FechaCreacion.ToString("dd/MM/yyyy");

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 26;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 36;
        ws.Column(4).Width = 20;
        ws.Column(5).Width = 14;
        ws.Column(6).Width = 18;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

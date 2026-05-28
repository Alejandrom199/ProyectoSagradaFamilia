namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class PadresPlantillaDocument : BaseExcelTemplate
{
    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Representantes");

        string[] headers = ["Nombre *", "Apellido *", "Email *", "Teléfono"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Plantilla de Importación de Representantes", totalCols);

        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Email es la clave única: si ya existe se actualizarán los datos   |   Si el email es nuevo se creará la cuenta automáticamente");

        EstilarEncabezadoColumnas(ws, 5, headers);

        ws.Cell(6, 1).Value = "María";
        ws.Cell(6, 2).Value = "González";
        ws.Cell(6, 3).Value = "maria.gonzalez@ejemplo.com";
        ws.Cell(6, 4).Value = "0999123456";
        EstilarFilaEjemplo(ws, 6, totalCols);

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 38;
        ws.Column(4).Width = 20;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class MedicosPlantillaDocument : BaseExcelTemplate
{
    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Medicos");

        string[] headers = ["Nombre *", "Apellido *", "Email *", "Teléfono", "Especialidad"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Plantilla de Importación de Médicos", totalCols);

        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Email es la clave única: si ya existe se actualizarán los datos   |   Si el email es nuevo se creará la cuenta automáticamente");

        EstilarEncabezadoColumnas(ws, 5, headers);

        ws.Cell(6, 1).Value = "Carlos";
        ws.Cell(6, 2).Value = "Ramírez";
        ws.Cell(6, 3).Value = "carlos.ramirez@ejemplo.com";
        ws.Cell(6, 4).Value = "0999987654";
        ws.Cell(6, 5).Value = "Pediatría";
        EstilarFilaEjemplo(ws, 6, totalCols);

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 38;
        ws.Column(4).Width = 20;
        ws.Column(5).Width = 28;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

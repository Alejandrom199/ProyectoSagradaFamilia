namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class MedidasPlantillaDocument : BaseExcelTemplate
{
    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Medidas");

        string[] headers = ["Nombre Paciente *", "Apellido Paciente *", "Fecha Medición *", "Peso kg *", "Talla cm *"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Plantilla de Importación de Medidas", totalCols);

        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Clave única: Paciente + Mes/Año   |   Si ya existe una medida en ese mes se actualizará   |   Fecha: yyyy-MM-dd   |   Paciente debe estar asignado al médico que importa");

        EstilarEncabezadoColumnas(ws, 5, headers);

        ws.Cell(6, 1).Value = "Sofia";
        ws.Cell(6, 2).Value = "Ramírez";
        ws.Cell(6, 3).Value = "2024-06-15";
        ws.Cell(6, 4).Value = 12.5;
        ws.Cell(6, 5).Value = 85.3;
        EstilarFilaEjemplo(ws, 6, totalCols);

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 22;
        ws.Column(4).Width = 14;
        ws.Column(5).Width = 14;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class NinosPlantillaDocument : BaseExcelTemplate
{
    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Pacientes");

        string[] headers = ["Nombre *", "Apellido *", "Email Representante *", "Sexo *", "Fecha Nacimiento *"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Plantilla de Importación de Pacientes", totalCols);

        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Clave única: Nombre + Apellido + Fecha Nacimiento   |   Si ya existe se actualizarán datos   |   Seleccione Masculino o Femenino   |   Fecha: yyyy-MM-dd");

        EstilarEncabezadoColumnas(ws, 5, headers);

        ws.Cell(6, 1).Value = "Sofia";
        ws.Cell(6, 2).Value = "Ramírez";
        ws.Cell(6, 3).Value = "carlos.ramirez@ejemplo.com";
        ws.Cell(6, 4).Value = "Femenino";
        ws.Cell(6, 5).Value = "2022-03-15";
        EstilarFilaEjemplo(ws, 6, totalCols);

        var dvSexo = ws.Range(ws.Cell(7, 4), ws.Cell(1048576, 4)).CreateDataValidation();
        dvSexo.List("\"Masculino,Femenino\"", true);
        dvSexo.ErrorStyle = XLErrorStyle.Stop;
        dvSexo.ErrorTitle = "Valor inválido";
        dvSexo.ErrorMessage = "Seleccione Masculino o Femenino del desplegable.";

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 38;
        ws.Column(4).Width = 12;
        ws.Column(5).Width = 22;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

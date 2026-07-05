namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class NinosPlantillaDocument : BaseExcelTemplate
{
    private readonly IEnumerable<(string NombreCompleto, string Email)> _representantes;

    public NinosPlantillaDocument(IEnumerable<(string NombreCompleto, string Email)> representantes)
    {
        _representantes = representantes;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Pacientes");

        string[] headers = ["Nombre *", "Apellido *", "Representante *", "Sexo *", "Fecha Nacimiento *"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Plantilla de Importación de Pacientes", totalCols);

        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Clave única: Nombre + Apellido + Fecha Nacimiento   |   Si ya existe se actualizarán datos   |   Representante: seleccione del desplegable   |   Sexo: Masculino o Femenino   |   Fecha: yyyy-MM-dd");

        EstilarEncabezadoColumnas(ws, 5, headers);

        var opcionesRepresentantes = _representantes
            .Select(r => $"{r.NombreCompleto} - {r.Email}")
            .ToList();

        ws.Cell(6, 1).Value = "Sofia";
        ws.Cell(6, 2).Value = "Ramírez";
        ws.Cell(6, 3).Value = opcionesRepresentantes.Count > 0
            ? opcionesRepresentantes[0]
            : "Carlos Ramírez - carlos.ramirez@ejemplo.com";
        ws.Cell(6, 4).Value = "Femenino";
        ws.Cell(6, 5).Value = "2022-03-15";
        EstilarFilaEjemplo(ws, 6, totalCols);

        var dvSexo = ws.Range(ws.Cell(7, 4), ws.Cell(1048576, 4)).CreateDataValidation();
        dvSexo.List("\"Masculino,Femenino\"", true);
        dvSexo.ErrorStyle = XLErrorStyle.Stop;
        dvSexo.ErrorTitle = "Valor inválido";
        dvSexo.ErrorMessage = "Seleccione Masculino o Femenino del desplegable.";

        if (opcionesRepresentantes.Count > 0)
        {
            var wsRepresentantes = workbook.Worksheets.Add("_Representantes");
            for (int i = 0; i < opcionesRepresentantes.Count; i++)
                wsRepresentantes.Cell(i + 1, 1).Value = opcionesRepresentantes[i];
            wsRepresentantes.Visibility = XLWorksheetVisibility.VeryHidden;

            var dvRepresentante = ws.Range(ws.Cell(7, 3), ws.Cell(1048576, 3)).CreateDataValidation();
            dvRepresentante.List(wsRepresentantes.Range(1, 1, opcionesRepresentantes.Count, 1), true);
            dvRepresentante.ErrorStyle = XLErrorStyle.Stop;
            dvRepresentante.ErrorTitle = "Representante inválido";
            dvRepresentante.ErrorMessage = "Seleccione un representante del listado desplegable.";
        }

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 45;
        ws.Column(4).Width = 12;
        ws.Column(5).Width = 22;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

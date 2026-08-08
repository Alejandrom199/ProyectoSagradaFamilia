namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class UsuariosPlantillaDocument : BaseExcelTemplate
{
    private readonly IEnumerable<(string NombreCompleto, string Email)> _medicos;
    private readonly IEnumerable<string> _especialidades;

    public UsuariosPlantillaDocument(
        IEnumerable<(string NombreCompleto, string Email)> medicos,
        IEnumerable<string> especialidades)
    {
        _medicos = medicos;
        _especialidades = especialidades;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Usuarios");

        string[] headers = ["Rol *", "Nombre *", "Apellido *", "Email *", "Teléfono", "Especialidad", "Médico"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Plantilla de Importación de Usuarios", totalCols);

        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Esta carga masiva solo CREA usuarios nuevos: si el email ya existe, la fila se rechaza   |   " +
            "Rol: Medico o Padre   |   Especialidad es obligatoria si Rol = Medico (seleccione del desplegable)   |   Médico es obligatorio solo si Rol = Padre (seleccione del desplegable)");

        EstilarEncabezadoColumnas(ws, 5, headers);

        var opcionesMedicos = _medicos
            .Select(m => $"{m.NombreCompleto} - {m.Email}")
            .ToList();
        var opcionesEspecialidades = _especialidades.ToList();

        ws.Cell(6, 1).Value = "Padre";
        ws.Cell(6, 2).Value = "Sofia";
        ws.Cell(6, 3).Value = "Ramírez";
        ws.Cell(6, 4).Value = "sofia.ramirez@ejemplo.com";
        ws.Cell(6, 5).Value = "0999987654";
        ws.Cell(6, 6).Value = string.Empty;
        ws.Cell(6, 7).Value = opcionesMedicos.Count > 0
            ? opcionesMedicos[0]
            : "Carlos Ramírez - carlos.ramirez@ejemplo.com";
        EstilarFilaEjemplo(ws, 6, totalCols);

        var dvRol = ws.Range(ws.Cell(7, 1), ws.Cell(1048576, 1)).CreateDataValidation();
        dvRol.List("\"Medico,Padre\"", true);
        dvRol.ErrorStyle = XLErrorStyle.Stop;
        dvRol.ErrorTitle = "Rol inválido";
        dvRol.ErrorMessage = "Seleccione Medico o Padre del desplegable.";

        if (opcionesEspecialidades.Count > 0)
        {
            var wsEspecialidades = workbook.Worksheets.Add("_Especialidades");
            for (int i = 0; i < opcionesEspecialidades.Count; i++)
                wsEspecialidades.Cell(i + 1, 1).Value = opcionesEspecialidades[i];
            wsEspecialidades.Visibility = XLWorksheetVisibility.VeryHidden;

            var dvEspecialidad = ws.Range(ws.Cell(7, 6), ws.Cell(1048576, 6)).CreateDataValidation();
            dvEspecialidad.List(wsEspecialidades.Range(1, 1, opcionesEspecialidades.Count, 1), true);
            dvEspecialidad.ErrorStyle = XLErrorStyle.Stop;
            dvEspecialidad.ErrorTitle = "Especialidad inválida";
            dvEspecialidad.ErrorMessage = "Seleccione una especialidad del listado desplegable.";
        }

        if (opcionesMedicos.Count > 0)
        {
            var wsMedicos = workbook.Worksheets.Add("_Medicos");
            for (int i = 0; i < opcionesMedicos.Count; i++)
                wsMedicos.Cell(i + 1, 1).Value = opcionesMedicos[i];
            wsMedicos.Visibility = XLWorksheetVisibility.VeryHidden;

            var dvMedico = ws.Range(ws.Cell(7, 7), ws.Cell(1048576, 7)).CreateDataValidation();
            dvMedico.List(wsMedicos.Range(1, 1, opcionesMedicos.Count, 1), true);
            dvMedico.ErrorStyle = XLErrorStyle.Stop;
            dvMedico.ErrorTitle = "Médico inválido";
            dvMedico.ErrorMessage = "Seleccione un médico del listado desplegable.";
        }

        ws.Column(1).Width = 12;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 26;
        ws.Column(4).Width = 34;
        ws.Column(5).Width = 18;
        ws.Column(6).Width = 24;
        ws.Column(7).Width = 45;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

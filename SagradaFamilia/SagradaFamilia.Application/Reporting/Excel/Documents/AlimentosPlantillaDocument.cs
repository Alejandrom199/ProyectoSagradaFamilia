namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;

public class AlimentosPlantillaDocument : BaseExcelTemplate
{
    private readonly IReadOnlyList<string> _categorias;

    public AlimentosPlantillaDocument(IReadOnlyList<string> categorias)
    {
        _categorias = categorias;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();

        // Hoja oculta: fuente del dropdown de categorías
        var catSheet = workbook.Worksheets.Add("Categorias");
        catSheet.Visibility = XLWorksheetVisibility.Hidden;
        for (int i = 0; i < _categorias.Count; i++)
            catSheet.Cell(i + 1, 1).Value = _categorias[i];

        var ws = workbook.Worksheets.Add("Alimentos");

        string[] headers = ["Nombre *", "Categoría *", "Edad Mínima (meses) *", "Descripción", "Recomendación"];
        int totalCols = headers.Length;

        // Filas 1-3: encabezado decorativo (logo + título + fecha)
        AgregarEncabezado(ws, "Plantilla de Importación de Alimentos", totalCols);

        // Fila 4: instrucciones
        AgregarFilaInstruccion(ws, 4, totalCols,
            "* = Campo obligatorio   |   Seleccione la categoría del desplegable   |   Edad mínima: número entero en meses (ej: 6, 12, 18)");

        // Fila 5: encabezados de columna
        EstilarEncabezadoColumnas(ws, 5, headers);

        // Fila 6: fila de ejemplo (referencia visual, no se importa)
        ws.Cell(6, 1).Value = "Manzana";
        ws.Cell(6, 2).Value = _categorias.FirstOrDefault() ?? "Frutas";
        ws.Cell(6, 3).Value = 6;
        ws.Cell(6, 4).Value = "Fruta suave, dulce y fácil de hacer puré";
        ws.Cell(6, 5).Value = "Ofrecer rallada o en puré sin cáscara";
        EstilarFilaEjemplo(ws, 6, totalCols);

        // Desde fila 7: zona de datos con validaciones
        if (_categorias.Count > 0)
        {
            var dvCat = ws.Range("B7:B1000").CreateDataValidation();
            dvCat.List(catSheet.Range($"A1:A{_categorias.Count}"), true);
            dvCat.ErrorTitle   = "Categoría inválida";
            dvCat.ErrorMessage = "Seleccione una categoría de la lista desplegable.";
        }

        var dvEdad = ws.Range("C7:C1000").CreateDataValidation();
        dvEdad.WholeNumber.Between(0, 240);
        dvEdad.ErrorTitle   = "Valor inválido";
        dvEdad.ErrorMessage = "Ingrese un número entero entre 0 y 240.";

        ws.Column(1).Width = 32;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 24;
        ws.Column(4).Width = 42;
        ws.Column(5).Width = 48;

        ws.SheetView.FreezeRows(6);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

namespace SagradaFamilia.Application.Reporting.Excel;

using System.Reflection;
using ClosedXML.Excel;

public abstract class BaseExcelTemplate
{
    // Paleta alineada con el design system del frontend
    protected const string ColorPrimario        = "#2563EB";
    protected const string ColorPrimarioOscuro  = "#1D4ED8";
    protected const string ColorTexto           = "#1E3A5F";
    protected const string ColorTextoSecundario = "#64748B";
    protected const string ColorTextoTenue      = "#94A3B8";
    protected const string ColorFondo           = "#F1F5F9";
    protected const string ColorBorde           = "#E2E8F0";
    protected const string ColorInstruccionFondo = "#FEF9C3";
    protected const string ColorInstruccionTexto = "#854D0E";

    protected const string NombreSistema = "Sagrada Familia — Sistema Pediátrico";

    public abstract byte[] GenerarBytes();

    // Bloque decorativo: logo (A1-A3 flotante) + título + subtítulo + fecha generación
    protected static void AgregarEncabezado(IXLWorksheet ws, string titulo, int totalColumnas)
    {
        try
        {
            using var logoStream = Assembly.GetAssembly(typeof(BaseExcelTemplate))!
                .GetManifestResourceStream("SagradaFamilia.Application.Assets.logo.png");
            if (logoStream != null)
                ws.AddPicture(logoStream).MoveTo(ws.Cell("A1")).WithSize(100, 55);
        }
        catch { /* continuar sin logo si no está disponible */ }

        string colFin = LetraColumna(totalColumnas);

        ws.Cell("B1").Value = titulo;
        ws.Range($"B1:{colFin}1").Merge();
        ws.Cell("B1").Style.Font.Bold = true;
        ws.Cell("B1").Style.Font.FontSize = 14;
        ws.Cell("B1").Style.Font.FontColor = XLColor.FromHtml(ColorTexto);
        ws.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("B1").Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;

        ws.Cell("B2").Value = NombreSistema;
        ws.Range($"B2:{colFin}2").Merge();
        ws.Cell("B2").Style.Font.Italic = true;
        ws.Cell("B2").Style.Font.FontColor = XLColor.FromHtml(ColorTextoSecundario);
        ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell("B3").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Range($"B3:{colFin}3").Merge();
        ws.Cell("B3").Style.Font.FontSize = 9;
        ws.Cell("B3").Style.Font.FontColor = XLColor.FromHtml(ColorTextoTenue);
        ws.Cell("B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Row(1).Height = 40;
        ws.Row(2).Height = 18;
        ws.Row(3).Height = 16;
    }

    // Fila amarilla de instrucciones (solo plantillas de importación)
    protected static void AgregarFilaInstruccion(IXLWorksheet ws, int fila, int totalColumnas, string texto)
    {
        string colFin = LetraColumna(totalColumnas);
        ws.Cell(fila, 1).Value = $"  {texto}";
        var rango = ws.Range($"A{fila}:{colFin}{fila}");
        rango.Merge();
        rango.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorInstruccionFondo);
        rango.Style.Font.FontColor  = XLColor.FromHtml(ColorInstruccionTexto);
        rango.Style.Font.FontSize   = 9;
        rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        rango.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
        ws.Row(fila).Height = 22;
    }

    // Fila de encabezados de columna: fondo azul primario, texto blanco, negrita
    protected static void EstilarEncabezadoColumnas(IXLWorksheet ws, int fila, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(fila, i + 1);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorPrimario);
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.FromHtml(ColorPrimarioOscuro);
        }
        ws.Row(fila).Height = 20;
    }

    // Fila de ejemplo para plantillas: fondo gris claro, cursiva, texto tenue
    protected static void EstilarFilaEjemplo(IXLWorksheet ws, int fila, int totalColumnas)
    {
        for (int i = 1; i <= totalColumnas; i++)
        {
            ws.Cell(fila, i).Style.Fill.BackgroundColor = XLColor.FromHtml(ColorFondo);
            ws.Cell(fila, i).Style.Font.Italic    = true;
            ws.Cell(fila, i).Style.Font.FontColor = XLColor.FromHtml(ColorTextoTenue);
        }
    }

    // Fila de dato con alternado de color (zebra striping)
    protected static void EstilarFilaDato(IXLWorksheet ws, int fila, int totalColumnas, bool esImpar)
    {
        for (int i = 1; i <= totalColumnas; i++)
        {
            ws.Cell(fila, i).Style.Fill.BackgroundColor = esImpar ? XLColor.White : XLColor.FromHtml(ColorFondo);
            ws.Cell(fila, i).Style.Border.BottomBorder      = XLBorderStyleValues.Thin;
            ws.Cell(fila, i).Style.Border.BottomBorderColor = XLColor.FromHtml(ColorBorde);
        }
    }

    // Serializa el workbook a byte array
    protected static byte[] GuardarComoBytes(XLWorkbook workbook)
    {
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    // Convierte índice de columna 1-based a letra de Excel: 1→A, 26→Z, 27→AA
    protected static string LetraColumna(int columna)
    {
        string resultado = string.Empty;
        while (columna > 0)
        {
            columna--;
            resultado = (char)('A' + columna % 26) + resultado;
            columna /= 26;
        }
        return resultado;
    }
}

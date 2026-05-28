namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class MedidasExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<MedidaDto.Response> _medidas;

    public MedidasExportDocument(IEnumerable<MedidaDto.Response> medidas)
    {
        _medidas = medidas;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Medidas");

        string[] headers = ["Paciente", "Fecha Medición", "Peso (kg)", "Talla (cm)", "Estado Nutricional", "Médico"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Listado de Medidas", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _medidas.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.NombreNino;
            ws.Cell(fila, 2).Value = item.FechaMedicion.ToString("yyyy-MM-dd");
            ws.Cell(fila, 3).Value = (double)item.Peso;
            ws.Cell(fila, 4).Value = (double)item.Talla;
            ws.Cell(fila, 5).Value = item.EstadoNutricional;
            ws.Cell(fila, 6).Value = item.NombreMedico;

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 30;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 14;
        ws.Column(4).Width = 14;
        ws.Column(5).Width = 24;
        ws.Column(6).Width = 30;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

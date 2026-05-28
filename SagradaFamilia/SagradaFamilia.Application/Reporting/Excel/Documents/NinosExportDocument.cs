namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class NinosExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<NinoDto.ListResponse> _ninos;

    public NinosExportDocument(IEnumerable<NinoDto.ListResponse> ninos)
    {
        _ninos = ninos;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Pacientes");

        string[] headers = ["Nombre", "Apellido", "Fecha Nacimiento", "Sexo", "Edad (meses)", "Representante"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Listado de Pacientes", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _ninos.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.Nombre;
            ws.Cell(fila, 2).Value = item.Apellido;
            ws.Cell(fila, 3).Value = item.FechaNacimiento.ToString("yyyy-MM-dd");
            ws.Cell(fila, 4).Value = item.Sexo == 'M' ? "Masculino" : "Femenino";
            ws.Cell(fila, 5).Value = item.EdadMeses;
            ws.Cell(fila, 6).Value = item.NombrePadre;

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 26;
        ws.Column(2).Width = 26;
        ws.Column(3).Width = 20;
        ws.Column(4).Width = 14;
        ws.Column(5).Width = 16;
        ws.Column(6).Width = 30;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

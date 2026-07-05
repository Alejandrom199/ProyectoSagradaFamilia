namespace SagradaFamilia.Application.Reporting.Excel.Documents;

using ClosedXML.Excel;
using SagradaFamilia.Application.DTOs;

public class UsuariosExportDocument : BaseExcelTemplate
{
    private readonly IEnumerable<UsuarioDto.ListResponse> _usuarios;

    public UsuariosExportDocument(IEnumerable<UsuarioDto.ListResponse> usuarios)
    {
        _usuarios = usuarios;
    }

    public override byte[] GenerarBytes()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Usuarios");

        string[] headers = ["Email", "Rol", "Activo", "Fecha Creación"];
        int totalCols = headers.Length;

        AgregarEncabezado(ws, "Listado de Usuarios", totalCols);
        EstilarEncabezadoColumnas(ws, 4, headers);

        var lista = _usuarios.ToList();
        for (int i = 0; i < lista.Count; i++)
        {
            int fila = i + 5;
            var item = lista[i];

            ws.Cell(fila, 1).Value = item.Email;
            ws.Cell(fila, 2).Value = item.RolNombre;
            ws.Cell(fila, 3).Value = item.Activo ? "Sí" : "No";
            ws.Cell(fila, 4).Value = item.FechaCreacion.ToString("yyyy-MM-dd");

            EstilarFilaDato(ws, fila, totalCols, i % 2 == 0);
        }

        ws.Column(1).Width = 36;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 12;
        ws.Column(4).Width = 18;

        ws.SheetView.FreezeRows(4);
        ws.SetTabActive();

        return GuardarComoBytes(workbook);
    }
}

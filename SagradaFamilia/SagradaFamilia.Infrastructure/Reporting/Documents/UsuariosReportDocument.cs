using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class UsuariosReportDocument : IDocument
    {
        private readonly IEnumerable<UsuarioDto.ListResponse> _usuarios;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string? _usuario;

        public UsuariosReportDocument(IEnumerable<UsuarioDto.ListResponse> usuarios, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            _usuarios = usuarios;
            _titulo = titulo ?? "Listado de Usuarios";
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            _usuario = usuario;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var template = new BaseReportTemplate(_titulo, _logoPath, _marcaAguaPath, true, GenerarTabla, _usuario);
            template.Compose(container);
        }

        private void GenerarTabla(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Email").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Rol").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Estado").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha Alta").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var u in _usuarios)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(u.Email).Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(u.RolNombre).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(u.Activo ? "Activo" : "Inactivo").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima)
                        .Text(u.FechaCreacion.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("es-EC")).ToLower())
                        .Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}

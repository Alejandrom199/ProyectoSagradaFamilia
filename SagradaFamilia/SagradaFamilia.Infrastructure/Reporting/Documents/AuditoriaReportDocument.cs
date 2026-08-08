using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class AuditoriaReportDocument : IDocument
    {
        private readonly IEnumerable<AuditoriaDto.Response> _registros;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string? _usuario;

        public AuditoriaReportDocument(IEnumerable<AuditoriaDto.Response> registros, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            _registros = registros;
            _titulo = titulo ?? "Registro de Actividad del Sistema";
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
            var cultura = new System.Globalization.CultureInfo("es-EC");

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Acción").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Módulo").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Usuario").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("IP").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var r in _registros)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(r.Fecha.ToString("dd MMM yyyy HH:mm", cultura).ToLower())
                        .Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(r.Accion).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(r.Tabla).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(r.UsuarioNombreCompleto ?? r.UsuarioEmail ?? "Sistema")
                        .Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(r.IpAddress ?? "-").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}

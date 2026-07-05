using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class LogsReportDocument : IDocument
    {
        private readonly IEnumerable<LogSistemaDto.Response> _logs;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string? _usuario;

        public LogsReportDocument(IEnumerable<LogSistemaDto.Response> logs, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            _logs = logs;
            _titulo = titulo ?? "Registro de Eventos del Sistema";
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
                    columns.RelativeColumn(7);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha y Hora").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Nivel").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Mensaje").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Endpoint").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var l in _logs)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(l.FechaHora.ToString("dd MMM yyyy HH:mm", cultura).ToLower())
                        .Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(l.Nivel).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(l.Mensaje).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(l.Endpoint ?? "—").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}

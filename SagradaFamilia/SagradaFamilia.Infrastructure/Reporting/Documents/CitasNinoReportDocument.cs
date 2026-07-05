using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class CitasNinoReportDocument : IDocument
    {
        private readonly IEnumerable<CitaDto.Response> _citas;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string? _usuario;

        public CitasNinoReportDocument(IEnumerable<CitaDto.Response> citas, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            _citas = citas;
            _titulo = titulo ?? "Historial de Citas";
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
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Paciente").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Médico").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Motivo").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Estado").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var c in _citas)
                {
                    var hora = c.FechaHora.ToString("HH:mm");
                    var horaFin = c.FechaHoraFin.HasValue ? $" – {c.FechaHoraFin.Value.ToString("HH:mm")}" : string.Empty;
                    var fechaTexto = $"{c.FechaHora.ToString("dd MMM yyyy", cultura).ToLower()} {hora}{horaFin}";

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(fechaTexto).Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(c.NombreNino).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text($"Dr(a). {c.NombreMedico}").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(c.Motivo ?? "—").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(c.Estado).Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}

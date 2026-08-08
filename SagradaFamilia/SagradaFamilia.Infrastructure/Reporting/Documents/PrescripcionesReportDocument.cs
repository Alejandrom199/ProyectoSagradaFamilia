using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;
using System.Linq;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class PrescripcionesReportDocument : IDocument
    {
        private readonly IEnumerable<PrescripcionDto.Response> _prescripciones;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string? _usuario;

        public PrescripcionesReportDocument(IEnumerable<PrescripcionDto.Response> prescripciones, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            _prescripciones = prescripciones;
            _titulo = titulo ?? "Historial de Prescripciones";
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
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Paciente").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Médico").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Medicamentos").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Indicaciones").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var p in _prescripciones)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(p.FechaCreacion.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("es-EC")).ToLower())
                        .Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(p.NombreNino).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text($"Dr(a). {p.NombreMedico}").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(string.Join(", ", p.Medicamentos.Select(m => $"{m.Nombre} ({m.Dosis}, {m.Frecuencia})")))
                        .Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(p.Indicaciones ?? "-").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}

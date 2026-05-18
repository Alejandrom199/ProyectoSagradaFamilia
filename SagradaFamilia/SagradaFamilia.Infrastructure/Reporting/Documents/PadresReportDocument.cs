using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;
using System.Collections.Generic;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class PadresReportDocument : IDocument
    {
        private readonly IEnumerable<PadreDto.ListResponse> _padres;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly bool isLandscape;

        public PadresReportDocument(
            IEnumerable<PadreDto.ListResponse> padres,
            string? titulo,
            string logoPath,
            string marcaAguaPath)
        {
            _padres = padres;
            _titulo = titulo ?? "Reporte General de Representantes";
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            isLandscape = true;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var template = new BaseReportTemplate(_titulo, _logoPath, _marcaAguaPath, isLandscape, GenerarTabla);
            template.Compose(container);
        }

        private void GenerarTabla(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(4); 
                    columns.RelativeColumn(3); 
                    columns.RelativeColumn(2); 
                    columns.RelativeColumn(2); 
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Nombre Completo").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Email").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Teléfono").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Hijos").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Estado").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var item in _padres)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{item.Nombre} {item.Apellido}").Style(BaseReportTemplate.EstiloValoresBold);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(item.Email ?? string.Empty).Style(BaseReportTemplate.EstiloValores);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(item.Telefono ?? string.Empty).Style(BaseReportTemplate.EstiloValores);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{item.TotalHijos}").Style(BaseReportTemplate.EstiloValores);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima)
                        .Text(item.Activo ? "Activo" : "Inactivo").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}
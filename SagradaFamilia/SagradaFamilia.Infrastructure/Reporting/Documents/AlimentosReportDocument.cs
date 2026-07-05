using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;
using System.Collections.Generic;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class AlimentosReportDocument : IDocument
    {
        private readonly IEnumerable<AlimentoDto.Response> _alimentos;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly bool isLandscape;
        private readonly string? _usuario;

        public AlimentosReportDocument(
            IEnumerable<AlimentoDto.Response> alimentos,
            string? titulo,
            string logoPath,
            string marcaAguaPath,
            string? usuario = null)
        {
            _alimentos = alimentos;
            _titulo = titulo ?? "Guía de Orientación Alimentaria";
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            isLandscape = true;
            _usuario = usuario;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var template = new BaseReportTemplate(_titulo, _logoPath, _marcaAguaPath, isLandscape, GenerarTabla, _usuario);
            template.Compose(container);
        }

        private void GenerarTabla(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(5);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Alimento").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Categoría").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Edad Mín.").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Recomendación").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var item in _alimentos)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(item.Nombre ?? string.Empty).Style(BaseReportTemplate.EstiloValoresBold);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(item.CategoriaNombre ?? string.Empty).Style(BaseReportTemplate.EstiloValores);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{item.EdadMinimaMeses} meses").Style(BaseReportTemplate.EstiloValores);

                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima)
                        .Text(item.Recomendacion ?? "N/A").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}
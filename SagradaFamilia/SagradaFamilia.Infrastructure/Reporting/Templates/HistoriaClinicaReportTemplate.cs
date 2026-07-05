using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace SagradaFamilia.Infrastructure.Reporting.Templates
{
    public class HistoriaClinicaReportTemplate
    {
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly Action<IContainer> _contentComposer;
        private readonly string? _usuarioGenerador;

        public HistoriaClinicaReportTemplate(
            string titulo,
            string logoPath,
            string marcaAguaPath,
            Action<IContainer> contentComposer,
            string? usuarioGenerador = null)
        {
            _titulo = titulo;
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            _contentComposer = contentComposer;
            _usuarioGenerador = usuarioGenerador;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Background()
                    .AlignCenter()
                    .AlignMiddle()
                    .Width(260)
                    .Image(_marcaAguaPath);

                page.Header().PaddingBottom(0.5f, Unit.Centimetre).Row(row =>
                {
                    row.ConstantItem(65).Image(_logoPath);
                    row.RelativeItem().PaddingLeft(12).Column(column =>
                    {
                        column.Item().Text("La Sagrada Familia").FontSize(18).Bold().FontColor(BaseReportTemplate.ColorNaval);
                        column.Item().Text(_titulo).FontSize(12).SemiBold().FontColor(BaseReportTemplate.ColorVerdePastel);
                    });
                    row.ConstantItem(140).AlignRight().AlignBottom().Column(col =>
                    {
                        col.Item().AlignRight().Text(DateTime.Now.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("es-EC")).ToLower())
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                        if (!string.IsNullOrWhiteSpace(_usuarioGenerador))
                            col.Item().AlignRight().Text($"Generado por: {_usuarioGenerador}")
                                .FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });

                // Sin marco exterior (a diferencia de BaseReportTemplate): este reporte
                // combina varias secciones apiladas y el borde redondeado se veía recargado.
                page.Content()
                    .PaddingTop(0.5f, Unit.Centimetre)
                    .Element(_contentComposer);

                page.Footer().PaddingTop(1, Unit.Centimetre).AlignCenter().Text(x =>
                {
                    x.Span("Página ").FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontColor(BaseReportTemplate.ColorNaval).Bold();
                    x.Span(" de ").FontColor(Colors.Grey.Medium);
                    x.TotalPages().FontColor(BaseReportTemplate.ColorNaval).Bold();
                });
            });
        }
    }
}

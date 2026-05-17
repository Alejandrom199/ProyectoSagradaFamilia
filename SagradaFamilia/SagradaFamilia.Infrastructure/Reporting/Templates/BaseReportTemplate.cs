using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace SagradaFamilia.Infrastructure.Reporting.Templates
{
    public class BaseReportTemplate
    {
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly bool _isLandscape;
        private readonly Action<IContainer> _contentComposer;

        // PALETA DE COLORES GLOBAL
        public static Color ColorNaval => Color.FromHex("#1B355A");
        public static Color ColorVerdePastel => Color.FromHex("#78A5A3");
        public static Color ColorTextoValores => Color.FromHex("#2D3748");
        public static Color ColorFondoCabecera => Color.FromHex("#1B355A");
        public static Color ColorBorde => Color.FromHex("#E2E8F0");

        // CONFIGURACIÓN DE PADDINGS Y MEDIDAS DE DISEÑO
        public static float PaddingCelda => 8;

        // TIPOGRAFÍAS Y ESTILOS DE TEXTO
        public static TextStyle EstiloTextoCabecera => TextStyle.Default.Bold().FontColor(Colors.White).FontSize(10);
        public static TextStyle EstiloValores => TextStyle.Default.FontColor(ColorTextoValores).FontSize(9.5f);
        public static TextStyle EstiloValoresBold => TextStyle.Default.Bold().FontColor(ColorNaval).FontSize(9.5f);

        public static IContainer EstiloCeldaCabecera(IContainer container)
        {
            return container
                .Background(ColorFondoCabecera)
                .Padding(PaddingCelda);
        }

        public static IContainer EstiloCeldaFila(IContainer container)
        {
            return container
                .BorderBottom(1).BorderColor(ColorBorde)
                .BorderRight(1).BorderColor(ColorBorde)
                .Padding(PaddingCelda);
        }

        public static IContainer EstiloCeldaFilaUltima(IContainer container)
        {
            return container
                .BorderBottom(1).BorderColor(ColorBorde)
                .Padding(PaddingCelda);
        }

        public BaseReportTemplate(
            string titulo,
            string logoPath,
            string marcaAguaPath,
            bool isLandscape,
            Action<IContainer> contentComposer)
        {
            _titulo = titulo;
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            _isLandscape = isLandscape;
            _contentComposer = contentComposer;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(_isLandscape ? PageSizes.A4.Landscape() : PageSizes.A4);
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
                        column.Item().Text("La Sagrada Familia").FontSize(18).Bold().FontColor(ColorNaval);
                        column.Item().Text(_titulo).FontSize(12).SemiBold().FontColor(ColorVerdePastel);
                    });
                    row.ConstantItem(100).AlignRight().AlignBottom().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                });

                page.Content()
                    .PaddingTop(0.5f, Unit.Centimetre)
                    .CornerRadius(6)
                    .Border(1)
                    .BorderColor(ColorNaval)
                    .Element(_contentComposer);

                page.Footer().PaddingTop(1, Unit.Centimetre).AlignCenter().Text(x =>
                {
                    x.Span("Página ").FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontColor(ColorNaval).Bold();
                    x.Span(" de ").FontColor(Colors.Grey.Medium);
                    x.TotalPages().FontColor(ColorNaval).Bold();
                });
            });
        }
    }
}
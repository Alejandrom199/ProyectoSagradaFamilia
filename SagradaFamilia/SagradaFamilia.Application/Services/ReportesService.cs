using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SagradaFamilia.Application.Services
{
    public class ReportesService : IReportesService
    {
        private readonly IAlimentoService _alimentoService;
        private readonly IPadreService _padreService; // Cambio: Usamos PadreService

        public ReportesService(IAlimentoService alimentoService, IPadreService padreService)
        {
            _alimentoService = alimentoService;
            _padreService = padreService;
        }

        public async Task<byte[]> GenerarAlimentosPdf(string? titulo)
        {
            var alimentos = await _alimentoService.ObtenerTodosAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // HEADER
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("La Sagrada Familia").FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                            column.Item().Text(titulo ?? "Guía de Orientación Alimentaria").FontSize(11).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    // CONTENIDO
                    page.Content().PaddingTop(0.8f, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Nombre
                            columns.RelativeColumn(3); // Categoría
                            columns.RelativeColumn(2); // Edad
                            columns.RelativeColumn(5); // Recomendación
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Alimento").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Categoría").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Edad Mín.").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Recomendación").Bold();
                        });

                        foreach (var item in alimentos)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(item.Nombre ?? string.Empty);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(item.CategoriaNombre ?? string.Empty);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text($"{item.EdadMinimaMeses}m");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(item.Recomendacion ?? "N/A");
                        }
                    });

                    page.Footer().AlignCenter().Text(x => {
                        x.Span("Página "); x.CurrentPageNumber(); x.Span(" de "); x.TotalPages();
                    });
                });
            });

            return pdf.GeneratePdf();
        }

        public async Task<byte[]> GenerarPadresPdf(string? titulo)
        {
            // Ahora le pedimos la lista al servicio correcto
            var padres = await _padreService.ObtenerTodosAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Text("Reporte General de Representantes").FontSize(16).Bold().FontColor(Colors.Blue.Medium);

                    page.Content().PaddingTop(0.5f, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); // Nombre Completo
                            columns.RelativeColumn(4); // Email
                            columns.RelativeColumn(2); // Teléfono
                            columns.RelativeColumn(1); // Hijos
                            columns.RelativeColumn(1); // Estado
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Nombre Completo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Email").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Teléfono").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Hijos").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Activo").Bold();
                        });

                        foreach (var item in padres)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.NombreCompleto ?? string.Empty);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Email ?? string.Empty);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Telefono ?? "-");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.TotalHijos.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Activo ? "S" : "N");
                        }
                    });

                    page.Footer().AlignCenter().Text(x => {
                        x.Span("Página "); x.CurrentPageNumber(); x.Span(" de "); x.TotalPages();
                    });
                });
            });

            return pdf.GeneratePdf();
        }
    }
}
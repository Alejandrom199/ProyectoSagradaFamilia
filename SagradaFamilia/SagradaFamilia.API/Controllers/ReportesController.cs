using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IAlimentoService _alimentoService;
        public record FilaReporteDto(string Fecha, string Peso, string Talla, string Estado);

        public ReportesController(IAlimentoService alimentoService)
        {
            _alimentoService = alimentoService;
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarReportePdf([FromQuery] string? titulo)
        {
            // En tu caso real, aquí consultas a tu base de datos mediante EF Core:
            // var datos = _context.Medidas
            //    .Where(m => m.NinoId == ninoId)
            //    .Select(m => new FilaReporteDto(m.FechaMedicion.ToString("yyyy-MM-dd"), $"{m.Peso} kg", $"{m.Talla} cm", m.EstadoNutricional))
            //    .ToList();

            var datosEjemplo = new List<FilaReporteDto>
        {
            new("2026-05-01", "9.65 kg", "77.2 cm", "Normal"),
            new("2026-04-15", "9.50 kg", "76.4 cm", "Normal"),
            new("2026-03-15", "9.25 kg", "75.2 cm", "Normal"),
            new("2026-02-15", "9.00 kg", "74.0 cm", "Normal")
        };

            // 2. Generación del documento con QuestPDF
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Encabezado del documento
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("La Sagrada Familia").FontSize(18).Bold().FontColor(Colors.Blue.Medium);
                            column.Item().Text(titulo ?? "Reporte de Crecimiento").FontSize(12).FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // Función local para estandarizar el diseño de cada celda
                    IContainer EstiloCelda(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);

                    // Contenido principal (Tabla de datos)
                    page.Content().PaddingTop(1, Unit.Centimetre).Table(table =>
                    {
                        // Definimos proporciones de las columnas usando RelativeColumn
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Fecha
                            columns.RelativeColumn(2); // Peso
                            columns.RelativeColumn(2); // Talla
                            columns.RelativeColumn(3); // Estado
                        });

                        // Encabezado de la tabla
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Peso").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Talla").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Estado").Bold();
                        });

                        // Cuerpo de la tabla (Filas)
                        foreach (var item in datosEjemplo)
                        {
                            table.Cell().Element(EstiloCelda).Text(item.Fecha);
                            table.Cell().Element(EstiloCelda).Text(item.Peso);
                            table.Cell().Element(EstiloCelda).Text(item.Talla);
                            table.Cell().Element(EstiloCelda).Text(item.Estado);
                        }
                    });

                    // Pie de página con numeración dinámica
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            // 3. Conversión y retorno del archivo PDF
            var pdfBytes = pdf.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"reporte_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public record FilaAlimentoDto(string Nombre, string Categoria, string EdadMinima, string Recomendacion);

        [HttpGet("alimentos-pdf")]
        public async Task<IActionResult> GenerarAlimentosPdf([FromQuery] string? titulo)
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

                    // Encabezado
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("La Sagrada Familia").FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                            column.Item().Text(titulo ?? "Guía de Orientación Alimentaria").FontSize(11).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    // Función de estilo para las celdas
                    IContainer EstiloCelda(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);

                    // Tabla de Alimentos
                    page.Content().PaddingTop(0.8f, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Nombre
                            columns.RelativeColumn(3); // Categoría
                            columns.RelativeColumn(2); // Edad Mínima
                            columns.RelativeColumn(5); // Recomendación
                        });

                        // Cabeceras de la tabla
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Alimento").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Categoría").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Edad Mínima").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Recomendación").Bold();
                        });

                        // Filas de datos
                        foreach (var item in alimentos)
                        {
                            table.Cell().Element(EstiloCelda).Text(item.Nombre ?? string.Empty);
                            table.Cell().Element(EstiloCelda).Text(item.CategoriaNombre ?? string.Empty);
                            table.Cell().Element(EstiloCelda).Text($"{item.EdadMinimaIntro} meses");
                            table.Cell().Element(EstiloCelda).Text(item.Recomendacion ?? "Sin recomendaciones específicas.");
                        }
                    });

                    // Footer del PDF
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            var pdfBytes = pdf.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"reporte-alimentos.pdf");
        }
    }
}
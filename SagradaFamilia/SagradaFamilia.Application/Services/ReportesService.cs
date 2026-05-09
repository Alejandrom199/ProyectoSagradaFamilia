using SagradaFamilia.Application.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SagradaFamilia.Application.Services
{
    public class ReportesService : IReportesService
    {
        private readonly IAlimentoService _alimentoService;
        private readonly IAuthService _authService;

        public ReportesService(IAlimentoService alimentoService, IAuthService authService)
        {
            _alimentoService = alimentoService;
            _authService = authService;
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

                    page.DefaultTextStyle(x =>
                        x.FontSize(10)
                         .FontFamily("Arial"));

                    // HEADER
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item()
                                .Text("La Sagrada Familia")
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            column.Item()
                                .Text(titulo ?? "Guía de Orientación Alimentaria")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);
                        });
                    });

                    // ESTILO CELDA
                    IContainer EstiloCelda(IContainer c) =>
                        c.BorderBottom(1)
                         .BorderColor(Colors.Grey.Lighten2)
                         .Padding(6);

                    // CONTENIDO
                    page.Content()
                        .PaddingTop(0.8f, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(5);
                            });

                            // HEADER TABLA
                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Alimento")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Categoría")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Edad Mínima")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Recomendación")
                                    .Bold();
                            });

                            // FILAS
                            foreach (var item in alimentos)
                            {
                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Nombre ?? string.Empty);

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.CategoriaNombre ?? string.Empty);

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text($"{item.EdadMinimaIntro} meses");

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Recomendacion ??
                                          "Sin recomendaciones específicas.");
                            }
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            return pdf.GeneratePdf();
        }

        public async Task<byte[]> GenerarPadresPdf(string? titulo)
        {
            var padres = await _authService.ObtenerPadresAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10)
                         .FontFamily("Arial"));

                    // HEADER
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item()
                                .Text("La Sagrada Familia")
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            column.Item()
                                .Text(titulo ?? "Reporte de Padres")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);
                        });
                    });

                    // ESTILO CELDA
                    IContainer EstiloCelda(IContainer c) =>
                        c.BorderBottom(1)
                         .BorderColor(Colors.Grey.Lighten2)
                         .Padding(6);

                    // TABLA
                    page.Content()
                        .PaddingTop(0.8f, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // ID
                                columns.RelativeColumn(2); // Nombre
                                columns.RelativeColumn(2); // Apellido
                                columns.RelativeColumn(3); // Email
                                columns.RelativeColumn(2); // Teléfono
                                columns.RelativeColumn(1); // Hijos
                                columns.RelativeColumn(1); // Estado
                            });

                            // HEADER TABLA
                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("ID")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Nombre")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Apellido")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Email")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Teléfono")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Hijos")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .Text("Activo")
                                    .Bold();
                            });

                            // FILAS
                            foreach (var item in padres)
                            {
                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Id.ToString());

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Nombre);

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Apellido);

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Email);

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Telefono ?? "N/A");

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.TotalHijos.ToString());

                                table.Cell()
                                    .Element(EstiloCelda)
                                    .Text(item.Activo ? "Sí" : "No");
                            }
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            return pdf.GeneratePdf();
        }
    }
}
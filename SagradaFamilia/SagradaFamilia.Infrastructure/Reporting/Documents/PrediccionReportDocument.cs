using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class PrediccionReportDocument : IDocument
    {
        private static readonly int[] MesesResumen = { 3, 6, 12 };
        private static readonly System.Globalization.CultureInfo Cultura = new("es-EC");

        private readonly NinoDto.DetailResponse _nino;
        private readonly PrediccionDto.Response _prediccion;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string _uid;
        private readonly string? _graficaPrediccionBase64;
        private readonly string? _graficaPrecisionBase64;
        private readonly string? _usuario;

        public PrediccionReportDocument(
            NinoDto.DetailResponse nino,
            PrediccionDto.Response prediccion,
            string? titulo, string logoPath, string marcaAguaPath, string uid,
            string? graficaPrediccionBase64, string? graficaPrecisionBase64, string? usuario = null)
        {
            _nino = nino;
            _prediccion = prediccion;
            _titulo = titulo ?? "Reporte de Predicción";
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            _uid = uid;
            _graficaPrediccionBase64 = graficaPrediccionBase64;
            _graficaPrecisionBase64 = graficaPrecisionBase64;
            _usuario = usuario;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var template = new BaseReportTemplate(_titulo, _logoPath, _marcaAguaPath, true, GenerarContenido, _usuario, _uid);
            template.Compose(container);
        }

        private void GenerarContenido(IContainer container)
        {
            container.Padding(10).Column(col =>
            {
                col.Item().Element(GenerarDatosPaciente);

                col.Item().PaddingTop(14).Text("Resumen de la predicción")
                    .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                col.Item().PaddingTop(4).Element(GenerarCardsResumen);

                if (!string.IsNullOrWhiteSpace(_graficaPrediccionBase64) || !string.IsNullOrWhiteSpace(_graficaPrecisionBase64))
                {
                    col.Item().PaddingTop(14).Text("Gráficas")
                        .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                    col.Item().PaddingTop(4).Element(GenerarGraficas);
                }

                col.Item().PaddingTop(14).Text("Desglose mes a mes")
                    .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                col.Item().PaddingTop(4).Element(GenerarTablaDesglose);
            });
        }

        private void GenerarDatosPaciente(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Paciente: {_nino.Nombre} {_nino.Apellido}").Style(BaseReportTemplate.EstiloValoresBold).FontSize(13);
                    col.Item().PaddingTop(4).Text($"Sexo: {(_nino.Sexo == 'M' ? "Masculino" : "Femenino")}").Style(BaseReportTemplate.EstiloValores);
                    col.Item().Text($"Fecha de nacimiento: {_nino.FechaNacimiento.ToString("dd MMM yyyy", Cultura).ToLower()}").Style(BaseReportTemplate.EstiloValores);
                });
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Representante").Style(BaseReportTemplate.EstiloValoresBold);
                    col.Item().PaddingTop(4).Text(_nino.PadreNombreCompleto).Style(BaseReportTemplate.EstiloValores);
                    col.Item().PaddingTop(8).Text("Médico tratante").Style(BaseReportTemplate.EstiloValoresBold);
                    col.Item().PaddingTop(4).Text(
                        string.IsNullOrWhiteSpace(_nino.MedicoEspecialidad)
                            ? $"Dr(a). {_nino.MedicoNombreCompleto}"
                            : $"Dr(a). {_nino.MedicoNombreCompleto} — {_nino.MedicoEspecialidad}")
                        .Style(BaseReportTemplate.EstiloValores);
                });
            });
        }

        private void GenerarCardsResumen(IContainer container)
        {
            var puntos = MesesResumen
                .Select(mes => _prediccion.Predicciones.FirstOrDefault(p => p.Meses == mes))
                .Where(p => p is not null)
                .ToList();

            if (puntos.Count == 0)
            {
                container.Text("Sin datos suficientes para el resumen.").Style(BaseReportTemplate.EstiloValores);
                return;
            }

            container.Row(row =>
            {
                foreach (var punto in puntos)
                {
                    row.RelativeItem().Padding(4).Border(1).BorderColor(BaseReportTemplate.ColorBorde).Padding(10).Column(col =>
                    {
                        col.Item().Text($"En {punto!.Meses} meses").Style(BaseReportTemplate.EstiloValores).FontSize(8);
                        col.Item().PaddingTop(2).Text($"{punto.PesoPredicho:0.00} kg").Bold().FontSize(16).FontColor(BaseReportTemplate.ColorNaval);
                        col.Item().Text($"Rango: {punto.PesoMinimo:0.00} – {punto.PesoMaximo:0.00} kg").Style(BaseReportTemplate.EstiloValores).FontSize(8);
                        col.Item().Text(punto.FechaObjetivo.ToString("dd MMM yyyy", Cultura).ToLower()).Style(BaseReportTemplate.EstiloValores).FontSize(7);
                    });
                }
            });
        }

        private void GenerarGraficas(IContainer container)
        {
            container.Row(row =>
            {
                if (!string.IsNullOrWhiteSpace(_graficaPrediccionBase64))
                {
                    var imagen = DecodificarImagenBase64(_graficaPrediccionBase64);
                    if (imagen is not null)
                    {
                        row.RelativeItem().Padding(6).Border(1).BorderColor(BaseReportTemplate.ColorBorde).Column(col =>
                        {
                            col.Item().Text("Curva de predicción").Style(BaseReportTemplate.EstiloValoresBold).FontSize(9);
                            col.Item().PaddingTop(4).Height(220).Image(imagen).FitArea();
                        });
                    }
                }

                if (!string.IsNullOrWhiteSpace(_graficaPrecisionBase64))
                {
                    var imagen = DecodificarImagenBase64(_graficaPrecisionBase64);
                    if (imagen is not null)
                    {
                        row.RelativeItem().Padding(6).Border(1).BorderColor(BaseReportTemplate.ColorBorde).Column(col =>
                        {
                            col.Item().Text("Precisión histórica").Style(BaseReportTemplate.EstiloValoresBold).FontSize(9);
                            col.Item().PaddingTop(4).Height(220).Image(imagen).FitArea();
                        });
                    }
                }
            });
        }

        private void GenerarTablaDesglose(IContainer container)
        {
            if (_prediccion.Predicciones.Count == 0)
            {
                container.Text("Sin registros.").Style(BaseReportTemplate.EstiloValores);
                return;
            }

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Mes").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha objetivo").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Peso predicho").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Rango de confianza").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Peso real").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var p in _prediccion.Predicciones.OrderBy(p => p.Meses))
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"Mes {p.Meses}").Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(p.FechaObjetivo.ToString("dd MMM yyyy", Cultura).ToLower()).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{p.PesoPredicho:0.00} kg").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{p.PesoMinimo:0.00} — {p.PesoMaximo:0.00} kg").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima)
                        .Text(p.PesoReal is not null ? $"{p.PesoReal:0.00} kg" : "Aún sin medir")
                        .Style(BaseReportTemplate.EstiloValores);
                }
            });
        }

        private static byte[]? DecodificarImagenBase64(string dataUri)
        {
            var comaIndex = dataUri.IndexOf(',');
            var base64 = comaIndex >= 0 ? dataUri[(comaIndex + 1)..] : dataUri;

            try
            {
                return Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}

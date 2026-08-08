using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class HistoriaClinicaReportDocument : IDocument
    {
        private readonly NinoDto.DetailResponse _nino;
        private readonly IEnumerable<MedidaDto.Response> _medidas;
        private readonly IEnumerable<ConsultaDto.Response> _consultas;
        private readonly IEnumerable<PrescripcionDto.Response> _prescripciones;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string _uid;
        private readonly string? _firmaMedicoBase64;
        private readonly string? _graficaCrecimientoBase64;
        private readonly string? _graficaImcBase64;
        private readonly string? _usuario;
        private static readonly System.Globalization.CultureInfo Cultura = new("es-EC");

        public HistoriaClinicaReportDocument(
            NinoDto.DetailResponse nino,
            IEnumerable<MedidaDto.Response> medidas,
            IEnumerable<ConsultaDto.Response> consultas,
            IEnumerable<PrescripcionDto.Response> prescripciones,
            string? titulo, string logoPath, string marcaAguaPath, string uid,
            string? firmaMedicoBase64 = null, string? graficaCrecimientoBase64 = null,
            string? graficaImcBase64 = null, string? usuario = null)
        {
            _nino = nino;
            _medidas = medidas;
            _consultas = consultas;
            _prescripciones = prescripciones;
            _titulo = titulo ?? "Historia Clínica";
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            _uid = uid;
            _firmaMedicoBase64 = firmaMedicoBase64;
            _graficaCrecimientoBase64 = graficaCrecimientoBase64;
            _graficaImcBase64 = graficaImcBase64;
            _usuario = usuario;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var template = new HistoriaClinicaReportTemplate(_titulo, _logoPath, _marcaAguaPath, GenerarContenido, _uid, _usuario);
            template.Compose(container);
        }

        private void GenerarContenido(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Element(GenerarDatosPaciente);

                col.Item().PaddingTop(14).Text("Historial de medidas")
                    .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                col.Item().PaddingTop(4).Element(GenerarTablaMedidas);

                if (!string.IsNullOrWhiteSpace(_graficaCrecimientoBase64) || !string.IsNullOrWhiteSpace(_graficaImcBase64))
                {
                    col.Item().PaddingTop(14).Text("Evolución clínica")
                        .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                    col.Item().PaddingTop(4).Element(GenerarGraficas);
                }

                col.Item().PaddingTop(14).Text("Historial de consultas")
                    .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                col.Item().PaddingTop(4).Element(GenerarTablaConsultas);

                col.Item().PaddingTop(14).Text("Prescripciones y medicamentos")
                    .Bold().FontSize(11).FontColor(BaseReportTemplate.ColorNaval);
                col.Item().PaddingTop(4).Element(GenerarTablaPrescripciones);

                if (!string.IsNullOrWhiteSpace(_firmaMedicoBase64))
                    col.Item().PaddingTop(28).Element(GenerarFirma);
            });
        }

        private void GenerarFirma(IContainer container)
        {
            var imagen = DecodificarImagenBase64(_firmaMedicoBase64!);
            if (imagen is null) return;

            container.Row(row =>
            {
                row.RelativeItem();
                row.ConstantItem(220).Column(col =>
                {
                    col.Item().Height(60).Image(imagen).FitArea();
                    col.Item().PaddingTop(2).BorderTop(1).BorderColor(BaseReportTemplate.ColorBorde);
                    col.Item().AlignCenter().PaddingTop(4).Text(
                        string.IsNullOrWhiteSpace(_nino.MedicoEspecialidad)
                            ? $"Dr(a). {_nino.MedicoNombreCompleto}"
                            : $"Dr(a). {_nino.MedicoNombreCompleto} - {_nino.MedicoEspecialidad}")
                        .Style(BaseReportTemplate.EstiloValoresBold).FontSize(9);
                    col.Item().AlignCenter().Text("Médico tratante").Style(BaseReportTemplate.EstiloValores).FontSize(8);
                });
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

        private void GenerarDatosPaciente(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Paciente: {_nino.Nombre} {_nino.Apellido}").Style(BaseReportTemplate.EstiloValoresBold).FontSize(13);
                    col.Item().PaddingTop(4).Text($"Sexo: {(_nino.Sexo == 'M' ? "Masculino" : "Femenino")}").Style(BaseReportTemplate.EstiloValores);
                    col.Item().Text($"Fecha de nacimiento: {_nino.FechaNacimiento.ToString("dd MMM yyyy", Cultura).ToLower()}").Style(BaseReportTemplate.EstiloValores);
                    col.Item().Text($"Edad: {FormatearEdad(_nino.EdadMeses)}").Style(BaseReportTemplate.EstiloValores);
                });
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Representante").Style(BaseReportTemplate.EstiloValoresBold);
                    col.Item().PaddingTop(4).Text(_nino.PadreNombreCompleto).Style(BaseReportTemplate.EstiloValores);
                    if (!string.IsNullOrWhiteSpace(_nino.PadreTelefono))
                        col.Item().Text($"Teléfono: {_nino.PadreTelefono}").Style(BaseReportTemplate.EstiloValores);
                    col.Item().PaddingTop(8).Text("Médico tratante").Style(BaseReportTemplate.EstiloValoresBold);
                    col.Item().PaddingTop(4).Text(
                        string.IsNullOrWhiteSpace(_nino.MedicoEspecialidad)
                            ? $"Dr(a). {_nino.MedicoNombreCompleto}"
                            : $"Dr(a). {_nino.MedicoNombreCompleto} - {_nino.MedicoEspecialidad}")
                        .Style(BaseReportTemplate.EstiloValores);
                });
            });
        }

        private static string FormatearEdad(int totalMeses)
        {
            var anios = totalMeses / 12;
            var meses = totalMeses % 12;

            var partes = new List<string>();
            if (anios > 0) partes.Add(anios == 1 ? "1 año" : $"{anios} años");
            if (meses > 0 || anios == 0) partes.Add(meses == 1 ? "1 mes" : $"{meses} meses");

            return string.Join(" y ", partes);
        }

        private void GenerarGraficas(IContainer container)
        {
            container.Row(row =>
            {
                if (!string.IsNullOrWhiteSpace(_graficaCrecimientoBase64))
                {
                    var imagen = DecodificarImagenBase64(_graficaCrecimientoBase64);
                    if (imagen is not null)
                    {
                        row.RelativeItem().Padding(6).Border(1).BorderColor(BaseReportTemplate.ColorBorde).Column(col =>
                        {
                            col.Item().Text("Curva de Crecimiento").Style(BaseReportTemplate.EstiloValoresBold).FontSize(9);
                            col.Item().PaddingTop(4).Image(imagen).FitWidth();
                        });
                    }
                }

                if (!string.IsNullOrWhiteSpace(_graficaImcBase64))
                {
                    var imagen = DecodificarImagenBase64(_graficaImcBase64);
                    if (imagen is not null)
                    {
                        row.RelativeItem().Padding(6).Border(1).BorderColor(BaseReportTemplate.ColorBorde).Column(col =>
                        {
                            col.Item().Text("Índice de Masa Corporal").Style(BaseReportTemplate.EstiloValoresBold).FontSize(9);
                            col.Item().PaddingTop(4).Image(imagen).FitWidth();
                        });
                    }
                }
            });
        }

        private void GenerarTablaMedidas(IContainer container)
        {
            if (!_medidas.Any())
            {
                container.Text("Sin registros.").Style(BaseReportTemplate.EstiloValores);
                return;
            }

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Peso (kg)").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Talla (cm)").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Estado Nutricional").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var m in _medidas)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(m.FechaMedicion.ToString("dd MMM yyyy", Cultura).ToLower())
                        .Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text($"{m.Peso} kg").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text($"{m.Talla} cm").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(m.EstadoNutricional).Style(BaseReportTemplate.EstiloValores);
                }
            });
        }

        private void GenerarTablaConsultas(IContainer container)
        {
            if (!_consultas.Any())
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
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Motivo").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Diagnóstico").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Indicaciones").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Evolución").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var c in _consultas)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(c.FechaCreacion.ToString("dd MMM yyyy", Cultura).ToLower())
                        .Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(c.Motivo ?? "-").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(c.Diagnostico ?? "-").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(c.Indicaciones ?? "-").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(c.Evolucion ?? "-").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }

        private void GenerarTablaPrescripciones(IContainer container)
        {
            if (!_prescripciones.Any())
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
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Médico").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Medicamentos").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Indicaciones").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var p in _prescripciones)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(p.FechaCreacion.ToString("dd MMM yyyy", Cultura).ToLower())
                        .Style(BaseReportTemplate.EstiloValoresBold);
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

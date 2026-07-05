using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class NinosReportDocument : IDocument
    {
        private readonly IEnumerable<NinoDto.ListResponse> _ninos;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly bool isLandscape;
        private readonly string? _usuario;

        public NinosReportDocument(
            IEnumerable<NinoDto.ListResponse> ninos,
            string? titulo,
            string logoPath,
            string marcaAguaPath,
            string? usuario = null
        )
        {
            _ninos = ninos;
            _titulo = titulo ?? "Reporte General de Pacientes";
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
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Paciente").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Edad Actual").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Representante").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Fecha de Nacimiento").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var item in _ninos)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{item.Nombre} {item.Apellido}" ?? string.Empty).Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{item.EdadMeses} meses").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text($"{item.NombrePadre}").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila)
                        .Text(item.FechaNacimiento.ToString("dd MMM yyyy", new System.Globalization.CultureInfo("es-EC")).ToLower())
                        .Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
        
    }
}

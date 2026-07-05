using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Infrastructure.Reporting.Templates;

namespace SagradaFamilia.Infrastructure.Reporting.Documents
{
    public class MedicosReportDocument : IDocument
    {
        private readonly IEnumerable<MedicoDto.ListResponse> _medicos;
        private readonly string _titulo;
        private readonly string _logoPath;
        private readonly string _marcaAguaPath;
        private readonly string? _usuario;

        public MedicosReportDocument(IEnumerable<MedicoDto.ListResponse> medicos, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            _medicos = medicos;
            _titulo = titulo ?? "Listado de Médicos";
            _logoPath = logoPath;
            _marcaAguaPath = marcaAguaPath;
            _usuario = usuario;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var template = new BaseReportTemplate(_titulo, _logoPath, _marcaAguaPath, true, GenerarTabla, _usuario);
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
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Médico").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Email").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Especialidad").Style(BaseReportTemplate.EstiloTextoCabecera);
                    header.Cell().Element(BaseReportTemplate.EstiloCeldaCabecera).Text("Teléfono").Style(BaseReportTemplate.EstiloTextoCabecera);
                });

                foreach (var m in _medicos)
                {
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text($"{m.Nombre} {m.Apellido}").Style(BaseReportTemplate.EstiloValoresBold);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(m.Email).Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFila).Text(m.Especialidad ?? "—").Style(BaseReportTemplate.EstiloValores);
                    table.Cell().Element(BaseReportTemplate.EstiloCeldaFilaUltima).Text(m.Telefono ?? "—").Style(BaseReportTemplate.EstiloValores);
                }
            });
        }
    }
}

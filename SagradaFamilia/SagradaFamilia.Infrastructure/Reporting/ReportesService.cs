using QuestPDF.Fluent;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Infrastructure.Reporting.Documents;

namespace SagradaFamilia.Infrastructure.Reporting
{
    public class ReportesService : IReportesService
    {
        private readonly IAlimentoService _alimentoService;
        private readonly IPadreService _padreService;
        private readonly INinoService _ninoService;

        public ReportesService(IAlimentoService alimentoService, IPadreService padreService, INinoService ninoService)
        {
            _alimentoService = alimentoService;
            _padreService = padreService;
            _ninoService = ninoService;
        }

        public async Task<byte[]> GenerarAlimentosPdf(string? titulo, string logoPath, string marcaAguaPath)
        {
            var alimentos = await _alimentoService.ObtenerTodosAsync();

            var documento = new AlimentosReportDocument(alimentos, titulo, logoPath, marcaAguaPath);

            return documento.GeneratePdf();
        }

        public async Task<byte[]> GenerarPadresPdf(string? titulo, string logoPath, string marcaAguaPath)
        {
            var padres = await _padreService.ObtenerTodosAsync();

            var documento = new PadresReportDocument(padres, titulo, logoPath, marcaAguaPath);

            return documento.GeneratePdf();
        }

        public async Task<byte[]> GenerarNinosPdf(string? titulo, string logoPath, string marcaAguaPath)
        {
            var ninos = await _ninoService.ObtenerTodosAsync();

            var documento = new NinosReportDocument(ninos, titulo, logoPath, marcaAguaPath);

            return documento.GeneratePdf();
        }
    }
}
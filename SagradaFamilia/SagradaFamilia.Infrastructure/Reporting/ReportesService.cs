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
        private readonly IMedicoService _medicoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IMedidaService _medidaService;
        private readonly IPrescripcionService _prescripcionService;
        private readonly ICitaService _citaService;
        private readonly IConsultaService _consultaService;
        private readonly ILogSistemaService _logService;
        private readonly IAuditoriaService _auditoriaService;

        public ReportesService(
            IAlimentoService alimentoService,
            IPadreService padreService,
            INinoService ninoService,
            IMedicoService medicoService,
            IUsuarioService usuarioService,
            IMedidaService medidaService,
            IPrescripcionService prescripcionService,
            ICitaService citaService,
            IConsultaService consultaService,
            ILogSistemaService logService,
            IAuditoriaService auditoriaService)
        {
            _alimentoService = alimentoService;
            _padreService = padreService;
            _ninoService = ninoService;
            _medicoService = medicoService;
            _usuarioService = usuarioService;
            _medidaService = medidaService;
            _prescripcionService = prescripcionService;
            _citaService = citaService;
            _consultaService = consultaService;
            _logService = logService;
            _auditoriaService = auditoriaService;
        }

        public async Task<byte[]> GenerarAlimentosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var alimentos = await _alimentoService.ObtenerTodosAsync();
            return new AlimentosReportDocument(alimentos, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarPadresPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var padres = await _padreService.ObtenerTodosAsync();
            return new PadresReportDocument(padres, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarNinosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var ninos = await _ninoService.ObtenerTodosAsync();
            return new NinosReportDocument(ninos, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarMedicosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var medicos = await _medicoService.ObtenerTodosAsync();
            return new MedicosReportDocument(medicos, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarUsuariosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return new UsuariosReportDocument(usuarios, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarMedidasPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var medidas = await _medidaService.ObtenerPorNinoAsync(ninoId);
            return new MedidasReportDocument(medidas, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarPrescripcionesPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var prescripciones = await _prescripcionService.ObtenerHistorialPorNinoAsync(ninoId);
            return new PrescripcionesReportDocument(prescripciones, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarCitasNinoPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var citas = await _citaService.ObtenerPorNinoIdAsync(ninoId);
            return new CitasNinoReportDocument(citas, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarCitasMedicoPdf(int usuarioId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var citas = await _citaService.ObtenerHistorialPorMedicoAsync(usuarioId);
            return new CitasNinoReportDocument(citas, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarPrescripcionesMedicoPdf(int usuarioId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var prescripciones = await _prescripcionService.ObtenerPorMedicoAsync(usuarioId);
            return new PrescripcionesReportDocument(prescripciones, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarHistoriaClinicaPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var nino = await _ninoService.ObtenerPorIdAsync(ninoId);
            var medidas = await _medidaService.ObtenerPorNinoAsync(ninoId);
            var consultas = await _consultaService.ObtenerHistorialPorNinoAsync(ninoId);
            var prescripciones = await _prescripcionService.ObtenerHistorialPorNinoAsync(ninoId);
            return new HistoriaClinicaReportDocument(nino, medidas, consultas, prescripciones, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarLogsPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var logs = await _logService.ObtenerRecientesAsync(500);
            return new LogsReportDocument(logs, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }

        public async Task<byte[]> GenerarAuditoriaPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null)
        {
            var registros = await _auditoriaService.ObtenerRecientesAsync(500);
            return new AuditoriaReportDocument(registros, titulo, logoPath, marcaAguaPath, usuario).GeneratePdf();
        }
    }
}

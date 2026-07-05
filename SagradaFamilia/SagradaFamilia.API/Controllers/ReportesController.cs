using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.Interfaces.Services;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SagradaFamilia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;
        private readonly IWebHostEnvironment _env;

        public ReportesController(IReportesService reportesService, IWebHostEnvironment env)
        {
            _reportesService = reportesService;
            _env = env;
        }

        private (string logo, string marca) Rutas() => (
            Path.Combine(_env.WebRootPath, "assets", "sagrada.png"),
            Path.Combine(_env.WebRootPath, "assets", "sagrada-opacity.png")
        );

        [HttpGet("alimentos-pdf")]
        public async Task<IActionResult> GenerarAlimentosPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarAlimentosPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-alimentos-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("padres-pdf")]
        public async Task<IActionResult> GenerarPadresPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarPadresPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-padres-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("ninos-pdf")]
        public async Task<IActionResult> GenerarNinosPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarNinosPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-pacientes-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("medicos-pdf")]
        public async Task<IActionResult> GenerarMedicosPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarMedicosPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-medicos-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("usuarios-pdf")]
        public async Task<IActionResult> GenerarUsuariosPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarUsuariosPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-usuarios-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("medidas-pdf")]
        public async Task<IActionResult> GenerarMedidasPdf([FromQuery] int ninoId, [FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarMedidasPdf(ninoId, titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-medidas-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("prescripciones-pdf")]
        public async Task<IActionResult> GenerarPrescripcionesPdf([FromQuery] int ninoId, [FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarPrescripcionesPdf(ninoId, titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-prescripciones-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("citas-nino-pdf")]
        public async Task<IActionResult> GenerarCitasNinoPdf([FromQuery] int ninoId, [FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarCitasNinoPdf(ninoId, titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-citas-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("citas-medico-pdf")]
        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> GenerarCitasMedicoPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarCitasMedicoPdf(usuarioId, titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-historial-citas-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("prescripciones-medico-pdf")]
        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> GenerarPrescripcionesMedicoPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarPrescripcionesMedicoPdf(usuarioId, titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-mis-prescripciones-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("historia-clinica-pdf")]
        [Authorize(Roles = "Medico,Administrador")]
        public async Task<IActionResult> GenerarHistoriaClinicaPdf([FromQuery] int ninoId, [FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarHistoriaClinicaPdf(ninoId, titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"historia-clinica-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("logs-pdf")]
        public async Task<IActionResult> GenerarLogsPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarLogsPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-logs-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("auditoria-pdf")]
        public async Task<IActionResult> GenerarAuditoriaPdf([FromQuery] string? titulo, [FromQuery] string? usuario)
        {
            var (logo, marca) = Rutas();
            var pdf = await _reportesService.GenerarAuditoriaPdf(titulo, logo, marca, usuario);
            return File(pdf, "application/pdf", $"reporte-auditoria-{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.Interfaces.Services;
using System.IO;
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

        [HttpGet("alimentos-pdf")]
        public async Task<IActionResult> GenerarAlimentosPdf([FromQuery] string? titulo)
        {
            string logoPath = Path.Combine(_env.WebRootPath, "assets", "sagrada.png");
            string marcaAguaPath = Path.Combine(_env.WebRootPath, "assets", "sagrada-opacity.png");

            var pdfBytes = await _reportesService.GenerarAlimentosPdf(titulo, logoPath, marcaAguaPath);

            return File(
                pdfBytes,
                "application/pdf",
                $"reporte-alimentos-{DateTime.Now:yyyyMMdd}.pdf"
            );
        }

        [HttpGet("padres-pdf")]
        public async Task<IActionResult> GenerarPadresPdf([FromQuery] string? titulo)
        {
            string logoPath = Path.Combine(_env.WebRootPath, "assets", "sagrada.png");
            string marcaAguaPath = Path.Combine(_env.WebRootPath, "assets", "sagrada-opacity.png");

            var pdfBytes = await _reportesService.GenerarPadresPdf(titulo, logoPath, marcaAguaPath);

            return File(
                pdfBytes,
                "application/pdf",
                $"reporte-padres-{DateTime.Now:yyyyMMdd}.pdf"
            );
        }

        [HttpGet("ninos-pdf")]
        public async Task<IActionResult> GenerarNinosPdf([FromQuery] string? titulo)
        {
            string logoPath = Path.Combine(_env.WebRootPath, "assets", "sagrada.png");
            string marcaAguaPath = Path.Combine(_env.WebRootPath, "assets", "sagrada-opacity.png");

            var pdfBytes = await _reportesService.GenerarNinosPdf(titulo, logoPath, marcaAguaPath);

            return File(
                pdfBytes,
                "application/pdf",
                $"reporte-padres-{DateTime.Now:yyyyMMdd}.pdf"
            );
        }
    }
}
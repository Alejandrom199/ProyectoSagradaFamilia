using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        [HttpGet("alimentos-pdf")]
        public async Task<IActionResult> GenerarAlimentosPdf([FromQuery] string? titulo)
        {
            var pdfBytes = await _reportesService.GenerarAlimentosPdf(titulo);

            return File(
                pdfBytes,
                "application/pdf",
                $"reporte-alimentos-{DateTime.Now:yyyyMMdd}.pdf"
            );
        }

        [HttpGet("padres-pdf")]
        public async Task<IActionResult> GenerarPadresPdf([FromQuery] string? titulo)
        {
            var pdfBytes = await _reportesService.GenerarPadresPdf(titulo);

            return File(
                pdfBytes,
                "application/pdf",
                $"reporte-padres-{DateTime.Now:yyyyMMdd}.pdf"
            );
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PadreController : BaseController
    {
        private readonly IPadreService _padreService;

        public PadreController(IPadreService padreService) => _padreService = padreService;

        [HttpGet]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PadreDto.ListResponse>>>> GetAll()
        {
            var result = await _padreService.ObtenerTodosAsync();
            return HandleResponse(result);
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<PagedResponse<PadreDto.ListResponse>>> GetPaginado(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] bool asc = true)
        {
            var result = await _padreService.ObtenerPaginadoAsync(page, pageSize, search, sortBy, asc);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<PadreDto.DetailResponse>>> GetById(int id)
        {
            var result = await _padreService.ObtenerPorIdAsync(id);
            return HandleResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<ApiResponse<PadreDto.DetailResponse>>> Create([FromBody] PadreDto.Create request)
        {
            var medicoIdClaim = User.FindFirst("medicoId")?.Value;

            if (!string.IsNullOrEmpty(medicoIdClaim))
            {
                request.MedicoId = int.Parse(medicoIdClaim);
            }
            else if (request.MedicoId <= 0)
            {
                return BadRequest(ApiResponse<PadreDto.DetailResponse>.Fail("Debe asignar un médico responsable."));
            }

            var result = await _padreService.CrearAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse<PadreDto.DetailResponse>.Ok(result, "Padre creado con éxito."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<ApiResponse<PadreDto.DetailResponse>>> Update(int id, [FromBody] PadreDto.Update request)
        {
            var medicoIdClaim = User.FindFirst("medicoId")?.Value;
            if (!string.IsNullOrEmpty(medicoIdClaim))
            {
                request.MedicoId = int.Parse(medicoIdClaim);
            }

            var result = await _padreService.ActualizarAsync(id, request);

            return Ok(ApiResponse<PadreDto.DetailResponse>.Ok(result, "Padre actualizado con éxito."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            await _padreService.EliminarAsync(id);
            return HandleSuccess("Padre eliminado.");
        }

        [HttpPatch("{id:int}/email")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<ApiResponse>> CambiarEmail(int id, [FromBody] PadreDto.ChangeEmail request)
        {
            await _padreService.CambiarEmailAsync(id, request.Email);
            return HandleSuccess("Correo electrónico actualizado correctamente.");
        }

        [HttpPost("{id:int}/reset-password")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(int id)
        {
            await _padreService.RestablecerPasswordAsync(id);
            return HandleSuccess("Se ha enviado un enlace de restablecimiento al correo del representante.");
        }

        [HttpGet("exportar")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<IActionResult> ExportarExcel()
        {
            var bytes = await _padreService.ExportarExcelAsync();
            string filename = $"representantes-{DateTime.Now:yyyyMMdd}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        [HttpGet("plantilla")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<IActionResult> DescargarPlantilla()
        {
            var bytes = await _padreService.GenerarPlantillaAsync();
            string filename = $"plantilla-representantes-{DateTime.Now:yyyyMMdd}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        [HttpPost("importar")]
        [Authorize(Roles = "Administrador, Medico")]
        public async Task<ActionResult<ApiResponse<PadreDto.ImportResultado>>> Importar([FromForm] IFormFile archivo)
        {
            if (archivo is null || archivo.Length == 0)
                return BadRequest(ApiResponse<PadreDto.ImportResultado>.Fail("No se recibió ningún archivo."));

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                return BadRequest(ApiResponse<PadreDto.ImportResultado>.Fail("Solo se aceptan archivos .xlsx"));

            var medicoIdClaim = User.FindFirst("medicoId")?.Value;
            if (!int.TryParse(medicoIdClaim, out int medicoId) || medicoId <= 0)
                return BadRequest(ApiResponse<PadreDto.ImportResultado>.Fail("No se pudo determinar el médico responsable."));

            using var stream = archivo.OpenReadStream();
            var resultado = await _padreService.ImportarAsync(stream, medicoId);

            string mensaje = resultado.Errores.Count == 0
                ? $"{resultado.Importados} creados, {resultado.Actualizados} actualizados correctamente."
                : $"{resultado.Importados} creados, {resultado.Actualizados} actualizados, {resultado.Errores.Count} con errores.";

            return HandleResponse(resultado, mensaje);
        }
    }
}
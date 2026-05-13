using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.API.Controllers
{
    [Authorize]
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            await _padreService.EliminarAsync(id);
            return HandleSuccess("Padre eliminado.");
        }
    }
}
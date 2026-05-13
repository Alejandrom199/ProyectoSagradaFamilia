using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Common;
using System.Security.Claims;

namespace SagradaFamilia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        // Obtiene el ID del usuario desde el Claim del JWT
        protected int UsuarioId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Obtiene el RolId personalizado que guardamos en el token
        protected int RolId => int.Parse(User.FindFirst("rolId")?.Value ?? "0");

        protected ActionResult<ApiResponse<T>> HandleResponse<T>(T data, string message = "Operación exitosa")
        {
            return Ok(ApiResponse<T>.Ok(data, message));
        }

        protected ActionResult<ApiResponse> HandleSuccess(string message = "Operación exitosa")
        {
            return Ok(ApiResponse.OkNoData(message));
        }
    }
}
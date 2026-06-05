using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;


namespace SagradaFamilia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<string> Health()
        {
            return "status: ok\nservice: sagrada-familia\nversion: 1.0";
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginDto.Response>>> Login([FromBody] LoginDto.Request request)
        {
            var response = await _authService.LoginAsync(request);

            SetTokenCookies(response.AccessToken, response.RefreshToken);

            return HandleResponse(response, "Sesión iniciada correctamente.");
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginDto.Response>>> RefreshToken([FromBody] RefreshTokenDto.Request request)
        {
            var response = await _authService.RefreshTokenAsync(request);

            SetTokenCookies(response.AccessToken, response.RefreshToken);

            return HandleResponse(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult<ApiResponse> Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return HandleSuccess("Sesión cerrada correctamente.");
        }

        [HttpPost("nueva-clave")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> NuevaClave([FromBody] NuevaClaveDto.Request request)
        {
            await _authService.NuevaClaveAsync(request);
            return HandleSuccess("Contraseña actualizada correctamente. Ya puedes iniciar sesión.");
        }

        [HttpPost("activar-cuenta")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> ActivarCuenta([FromBody] ActivarCuentaDto.Request request)
        {
            await _authService.ActivarCuentaAsync(request);
            return HandleSuccess("Cuenta activada correctamente. Ya puedes iniciar sesión.");
        }

        [HttpPost("solicitar-reset")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> SolicitarReset([FromBody] SolicitarResetDto.Request request)
        {
            await _authService.SolicitarResetAsync(request);
            return HandleSuccess("Si el correo corresponde a una cuenta médica activa, recibirás un enlace en unos minutos.");
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Cambiar a true en producción (HTTPS)
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });

            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }

}
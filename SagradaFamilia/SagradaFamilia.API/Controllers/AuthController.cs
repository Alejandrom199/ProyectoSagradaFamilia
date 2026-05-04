namespace SagradaFamilia.API.Controllers;

using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) =>
        _authService = authService;

    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Health()
    {
        return "status: ok\nservice: sagrada-familia\nversion: 1.0";
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        SetTokenCookies(response.AccessToken, response.RefreshToken);

        return Ok(ApiResponse<LoginResponse>.Ok(response));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken(
        [FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        return Ok(ApiResponse<LoginResponse>.Ok(response));
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok(ApiResponse<object>.Ok(new { mensaje = "Sesión cerrada." }));
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    [HttpPost("padres")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<UsuarioResponse>>> CrearPadre(
        [FromBody] CrearPadreRequest request)
    {
        var response = await _authService.CrearPadreAsync(request);
        return CreatedAtAction(nameof(ObtenerPadres),
            ApiResponse<UsuarioResponse>.Ok(response, "Padre creado exitosamente."));
    }

    [HttpGet("padres")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioResponse>>>> ObtenerPadres()
    {
        var response = await _authService.ObtenerPadresAsync();
        return Ok(ApiResponse<IEnumerable<UsuarioResponse>>.Ok(response));
    }

    [HttpDelete("padres/{id:int}")]
    [Authorize(Roles = "Medico")]
    public async Task<ActionResult<ApiResponse>> EliminarPadre(int id)
    {
        await _authService.EliminarPadreAsync(id);
        return Ok(ApiResponse.OkNoData("Padre eliminado exitosamente."));
    }
}
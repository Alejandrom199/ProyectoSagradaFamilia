using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SagradaFamilia.Infrastructure.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration) => _configuration = configuration;

        public string GenerarAccessToken(Usuario usuario)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"]!;
            var issuer = _configuration["JwtSettings:Issuer"]!;
            var audience = _configuration["JwtSettings:Audience"]!;
            var minutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, ObtenerNombreCompleto(usuario)),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Sin Rol"),
                new Claim("rolId", usuario.RolId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (usuario.Medico != null)
            {
                claims.Add(new Claim("medicoId", usuario.Medico.Id.ToString()));
            }

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string GenerarRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public DateTime ObtenerFechaExpiracionRefreshToken()
        {
            var days = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
            return DateTime.UtcNow.AddDays(days);
        }

        private string ObtenerNombreCompleto(Usuario usuario)
        {
            if (usuario.Medico != null)
                return $"{usuario.Medico.Nombre} {usuario.Medico.Apellido}".Trim();

            if (usuario.Padre != null)
                return $"{usuario.Padre.Nombre} {usuario.Padre.Apellido}".Trim();

            if (usuario.Rol?.Nombre == "Administrador")
                return "Administrador del Sistema";

            return "Usuario Sagrada Familia";
        }
    }
}

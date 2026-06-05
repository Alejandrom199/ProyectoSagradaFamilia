using System.Text.Json.Serialization;

namespace SagradaFamilia.Application.DTOs.Auth
{
    public static class LoginDto
    {
        public class Request
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class Response
        {
            [JsonIgnore]
            public string AccessToken { get; set; } = string.Empty;
            [JsonIgnore]
            public string RefreshToken { get; set; } = string.Empty;
            public int Id { get; set; }
            public int? MedicoId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Rol { get; set; } = string.Empty;
            public DateTime Expiracion { get; set; }
        }
    }
}
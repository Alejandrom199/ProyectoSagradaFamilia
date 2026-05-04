using System.Text.Json.Serialization;

namespace SagradaFamilia.Application.DTOs.Auth
{
    public class LoginResponse
    {
        [JsonIgnore]
        public string AccessToken { get; set; } = string.Empty;
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
    }
}

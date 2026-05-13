namespace SagradaFamilia.Application.DTOs.Auth
{
    public static class RefreshTokenDto
    {
        public class Request
        {
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
}
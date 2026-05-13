namespace SagradaFamilia.Application.DTOs.Auth
{
    public static class PermisoDto
    {
        public class AsignarRequest
        {
            public int UsuarioId { get; set; }
            public int OpcionAccionId { get; set; }
            public bool Permitido { get; set; } = true;
        }
    }
}
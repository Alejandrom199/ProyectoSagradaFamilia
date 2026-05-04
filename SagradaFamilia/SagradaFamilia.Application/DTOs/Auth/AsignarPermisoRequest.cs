namespace SagradaFamilia.Application.DTOs.Auth
{
    public class AsignarPermisoRequest
    {
        public int UsuarioId { get; set; }
        public int OpcionAccionId { get; set; }
        public bool Permitido { get; set; } = true;
    }
}

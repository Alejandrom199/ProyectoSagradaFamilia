using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Revocado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}

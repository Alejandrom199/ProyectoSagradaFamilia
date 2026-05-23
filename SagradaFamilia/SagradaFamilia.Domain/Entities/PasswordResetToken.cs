using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public int UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; } = false;
        public DateTime? FechaUso { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public Usuario Usuario { get; set; } = null!;
    }
}

using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Entities
{
    public class Usuario : AuditableEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string? Telefono { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public Rol Rol { get; set; } = null!;
        public ICollection<Nino> Ninos { get; set; } = new List<Nino>();
        public ICollection<Medida> Medidas { get; set; } = new List<Medida>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UsuarioPermiso> Permisos { get; set; } = new List<UsuarioPermiso>();
    }
}

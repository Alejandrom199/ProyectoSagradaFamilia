using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Usuario : AuditableEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Activo { get; set; } = true;

        public Rol Rol { get; set; } = null!;
        public Padre? Padre { get; set; }
        public Medico? Medico { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UsuarioPermiso> Permisos { get; set; } = new List<UsuarioPermiso>();
        
    }
}
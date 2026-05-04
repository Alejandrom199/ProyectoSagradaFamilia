using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Rol : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty; // Medico, Padre

        // Navegación
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}

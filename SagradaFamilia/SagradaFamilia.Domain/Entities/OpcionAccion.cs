using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Entities
{
    public class OpcionAccion : BaseEntity
    {
        public int OpcionId { get; set; }
        public int AccionId { get; set; } 

        // Navegación
        public Opcion Opcion { get; set; } = null!;
        public Accion Accion { get; set; } = null!;
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
        public ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = new List<UsuarioPermiso>();
    }
}

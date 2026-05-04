using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Accion : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty; // Ver, Crear, Editar, Eliminar

        // Navegación
        public ICollection<OpcionAccion> OpcionAcciones { get; set; } = new List<OpcionAccion>();
    }
}

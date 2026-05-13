using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Accion : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;

        // Navegación
        public ICollection<OpcionAccion> OpcionAcciones { get; set; } = new List<OpcionAccion>();
    }
}

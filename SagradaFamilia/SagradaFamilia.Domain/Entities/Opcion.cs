using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Opcion : AuditableEntity
    {
        public int ModuloId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ruta { get; set; }
        public string? Icono { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public Modulo Modulo { get; set; } = null!;
        public ICollection<OpcionAccion> OpcionAcciones { get; set; } = new List<OpcionAccion>();
    }
}

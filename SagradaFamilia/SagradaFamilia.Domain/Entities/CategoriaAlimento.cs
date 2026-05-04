using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class CategoriaAlimento : AuditableEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Alimento> Alimentos { get; set; } = new List<Alimento>();
    }
}

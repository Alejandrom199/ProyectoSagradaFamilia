using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Alimento : AuditableEntity
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int EdadMinimaMeses { get; set; }
        public string? Recomendacion { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public CategoriaAlimento Categoria { get; set; } = null!;
    }
}

namespace SagradaFamilia.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public int UsuarioCreacionId { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        public int? UsuarioEliminacionId { get; set; }
    }
}
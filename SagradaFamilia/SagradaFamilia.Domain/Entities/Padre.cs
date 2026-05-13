using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Padre : AuditableEntity
    {
        public int UsuarioId { get; set; }
        public int MedicoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public ICollection<Nino> Ninos { get; set; } = new List<Nino>();
    }
}
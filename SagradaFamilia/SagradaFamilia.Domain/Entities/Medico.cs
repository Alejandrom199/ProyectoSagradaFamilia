using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Medico : AuditableEntity
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Especialidad { get; set; }
        public string? FirmaImagen { get; set; }
        public DateTime? FirmaActualizadaEn { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public ICollection<Padre> Padres { get; set; } = new List<Padre>();
        public ICollection<Nino> Ninos { get; set; } = new List<Nino>();
        public ICollection<Medida> Medidas { get; set; } = new List<Medida>();
    }
}
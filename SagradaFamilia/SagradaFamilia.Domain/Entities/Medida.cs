using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Medida : AuditableEntity
    {
        public int NinoId { get; set; }
        public int MedicoId { get; set; }
        public DateOnly FechaMedicion { get; set; }  // va a Prophet como ds
        public decimal Peso { get; set; }  // en kg
        public decimal Talla { get; set; }  // en cm
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación
        public Nino Nino { get; set; } = null!;
        public Usuario Medico { get; set; } = null!;
    }
}

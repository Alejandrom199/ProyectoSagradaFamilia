using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Medicamento : AuditableEntity
    {
        public int PrescripcionId { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? Presentacion { get; set; }
        public string Dosis { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public string ViaAdministracion { get; set; } = string.Empty;
        public string? Duracion { get; set; }
        public string? Cantidad { get; set; }
        public string? Observaciones { get; set; }

        // Navegación
        public Prescripcion Prescripcion { get; set; } = null!;
    }
}

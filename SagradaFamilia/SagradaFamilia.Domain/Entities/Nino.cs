using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Nino : AuditableEntity
    {
        public int PadreId { get; set; }
        public int MedicoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public char Sexo { get; set; }

        // Navegación
        public Padre Padre { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public ICollection<Medida> Medidas { get; set; } = new List<Medida>();
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public ICollection<Prediccion> Predicciones { get; set; } = new List<Prediccion>();
    }
}
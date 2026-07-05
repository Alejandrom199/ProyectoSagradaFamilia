using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;
using System.Collections.Generic;

namespace SagradaFamilia.Domain.Entities
{
    public class Consulta : AuditableEntity
    {
        public int CitaId { get; set; }
        public int NinoId { get; set; }
        public int MedicoId { get; set; }

        public string? Motivo { get; set; }
        public string? Diagnostico { get; set; }
        public string? Indicaciones { get; set; }
        public string? Evolucion { get; set; }

        public EstadoConsulta Estado { get; set; } = EstadoConsulta.EnCurso;

        // Navegación
        public Cita Cita { get; set; } = null!;
        public Nino Nino { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public ICollection<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
    }
}

using SagradaFamilia.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Entities
{
    public class Prescripcion : AuditableEntity
    {
        public int NinoId { get; set; }
        public int MedicoId { get; set; }
        public int CitaId { get; set; }

        public string? Diagnostico { get; set; }
        public string DetalleMedicamentos { get; set; } = string.Empty;
        public string? Indicaciones { get; set; }

        // Navegación
        public Nino Nino { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public Cita Cita { get; set; } = null!;
    }
}

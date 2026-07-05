using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Entities
{
    public class Cita : AuditableEntity
    {
        public int NinoId { get; set; }
        public int MedicoId { get; set; }

        public DateTime FechaHora    { get; set; }
        public DateTime? FechaHoraFin { get; set; }  // null en registros anteriores → se asume 30 min
        public string? Motivo { get; set; }

        public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
        public string? NotasConsulta { get; set; }
        public string? MotivoCancelacion { get; set; }

        // Navegación
        public Nino Nino { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public Consulta? Consulta { get; set; }
    }
}

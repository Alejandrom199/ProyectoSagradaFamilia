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
        public int ConsultaId { get; set; }

        public string? Indicaciones { get; set; }

        // Navegación
        public Nino Nino { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public Consulta Consulta { get; set; } = null!;
        public ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    }
}

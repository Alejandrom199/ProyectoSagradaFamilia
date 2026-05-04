using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
    }
}

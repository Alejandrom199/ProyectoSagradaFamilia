using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Entities
{
    public class RolPermiso : BaseEntity
    {
        public int RolId { get; set; }
        public int OpcionAccionId { get; set; }
        public bool Permitido { get; set; } = true;

        // Navegación
        public Rol Rol { get; set; } = null!;
        public OpcionAccion OpcionAccion { get; set; } = null!;
    }
}

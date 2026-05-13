using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Entities
{
    public class Modulo : AuditableEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Icono { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Opcion> Opciones { get; set; } = new List<Opcion>();
    }
}

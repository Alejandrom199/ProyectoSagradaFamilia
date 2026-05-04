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
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }  // nombre del icono para Angular
        public int Orden { get; set; }  // orden en el menú
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Opcion> Opciones { get; set; } = new List<Opcion>();
    }
}

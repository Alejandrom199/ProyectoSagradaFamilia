using SagradaFamilia.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Entities
{
    public class Nino : AuditableEntity
    {
        public int RepresentanteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public char Sexo { get; set; }  // 'M' o 'F'

        // Navegación
        public Usuario Representante { get; set; } = null!;
        public ICollection<Medida> Medidas { get; set; } = new List<Medida>();
        public ICollection<Prediccion> Predicciones { get; set; } = new List<Prediccion>();
    }
}

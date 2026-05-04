using SagradaFamilia.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Entities
{
    public class LogSistema : BaseEntity
    {
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
        public string Nivel { get; set; } = string.Empty; // Information, Warning, Error
        public string Mensaje { get; set; } = string.Empty;
        public string? Excepcion { get; set; }
        public string? StackTrace { get; set; }
        public string? Endpoint { get; set; } // qué endpoint generó el log
        public int? UsuarioId { get; set; } // quién estaba logueado
        public string? IpAddress { get; set; }
    }
}

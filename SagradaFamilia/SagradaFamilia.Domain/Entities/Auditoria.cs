using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Auditoria : BaseEntity
    {
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string Accion { get; set; } = string.Empty;
        public string Tabla { get; set; } = string.Empty;
        public string ClavePrimaria { get; set; } = string.Empty;

        public string? ValoresAntiguos { get; set; }
        public string? ValoresNuevos { get; set; }

        public string? IpAddress { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}

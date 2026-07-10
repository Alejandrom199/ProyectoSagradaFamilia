using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class UsuarioEstadoHistorial : BaseEntity
    {
        public int UsuarioId { get; set; }
        public bool EstadoNuevo { get; set; }
        public string? Motivo { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
        public int UsuarioQueRealizoCambioId { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Usuario UsuarioQueRealizoCambio { get; set; } = null!;
    }
}

using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class UsuarioPermiso : BaseEntity
    {
        public int UsuarioId { get; set; }
        public int OpcionAccionId { get; set; }
        public bool Permitido { get; set; } = true;

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public OpcionAccion OpcionAccion { get; set; } = null!;
    }
}

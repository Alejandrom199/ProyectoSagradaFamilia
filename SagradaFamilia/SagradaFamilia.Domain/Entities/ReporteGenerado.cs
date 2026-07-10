using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Entities
{
    public class ReporteGenerado : BaseEntity
    {
        public string Uid { get; set; } = string.Empty;
        public TipoReporte Tipo { get; set; }
        public int NinoId { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
        public int UsuarioGeneradorId { get; set; }

        // Navegación
        public Nino Nino { get; set; } = null!;
        public Usuario UsuarioGenerador { get; set; } = null!;
    }
}

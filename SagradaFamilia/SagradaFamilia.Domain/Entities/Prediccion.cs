using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Entities
{
    public class Prediccion : BaseEntity
    {
        public int NinoId { get; set; }
        public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;
        public DateOnly FechaObjetivo { get; set; }
        public int ProyeccionMeses { get; set; }
        public TipoReferencia Tipo { get; set; }

        public decimal ValorPredicho { get; set; }   // yhat
        public decimal ValorMinimo { get; set; }   // yhat_lower
        public decimal ValorMaximo { get; set; }   // yhat_upper
        public decimal? ValorReal { get; set; }   // null hasta que llegue la fecha

        // Navegación
        public Nino Nino { get; set; } = null!;
    }
}

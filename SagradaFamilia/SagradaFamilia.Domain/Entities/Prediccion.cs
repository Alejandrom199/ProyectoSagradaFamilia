using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class Prediccion : BaseEntity
    {
        public int NinoId { get; set; }
        public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;
        public DateOnly FechaObjetivo { get; set; }
        public int Meses { get; set; }   // 3, 6 o 12
        public decimal PesoPredicho { get; set; }   // yhat
        public decimal PesoMinimo { get; set; }   // yhat_lower
        public decimal PesoMaximo { get; set; }   // yhat_upper
        public decimal? PesoReal { get; set; }   // null hasta que llegue la fecha

        // Navegación
        public Nino Nino { get; set; } = null!;
    }
}

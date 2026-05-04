namespace SagradaFamilia.Application.DTOs.Predicciones
{
    public class PuntoPrediccion
    {
        public int Meses { get; set; }
        public DateOnly FechaObjetivo { get; set; }
        public decimal PesoPredicho { get; set; }
        public decimal PesoMinimo { get; set; }
        public decimal PesoMaximo { get; set; }
        public decimal? PesoReal { get; set; }
    }
}

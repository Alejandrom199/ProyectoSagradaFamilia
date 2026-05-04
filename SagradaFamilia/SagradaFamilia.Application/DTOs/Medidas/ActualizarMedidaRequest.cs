namespace SagradaFamilia.Application.DTOs.Medidas
{
    public class ActualizarMedidaRequest
    {
        public DateOnly FechaMedicion { get; set; }
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
    }
}
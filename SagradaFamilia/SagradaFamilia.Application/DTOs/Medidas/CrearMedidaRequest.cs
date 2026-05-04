namespace SagradaFamilia.Application.DTOs.Medidas
{
    public class CrearMedidaRequest
    {
        public int NinoId { get; set; }
        public DateOnly FechaMedicion { get; set; }
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
    }
}

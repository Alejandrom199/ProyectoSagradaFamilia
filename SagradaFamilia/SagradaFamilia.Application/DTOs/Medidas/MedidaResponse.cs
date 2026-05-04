namespace SagradaFamilia.Application.DTOs.Medidas
{
    public class MedidaResponse
    {
        public int Id { get; set; }
        public int NinoId { get; set; }
        public string NombreNino { get; set; } = string.Empty;
        public DateOnly FechaMedicion { get; set; }
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public string EstadoNutricional { get; set; } = string.Empty;
        public int Percentil { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}

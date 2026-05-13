namespace SagradaFamilia.Application.DTOs
{
    public static class PrediccionDto
    {
        public class Response
        {
            public bool PuedePredecir { get; set; }
            public string? Mensaje { get; set; }
            public int NinoId { get; set; }
            public List<Punto> Predicciones { get; set; } = new();
        }

        public class Punto
        {
            public int Meses { get; set; }
            public DateOnly FechaObjetivo { get; set; }
            public decimal PesoPredicho { get; set; }
            public decimal PesoMinimo { get; set; }
            public decimal PesoMaximo { get; set; }
            public decimal? PesoReal { get; set; }
        }

        public class Health
        {
            public string Status { get; set; } = string.Empty;
            public string Service { get; set; } = string.Empty;
            public string? Version { get; set; } = string.Empty;
        }
    }
}
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

        /// <summary>
        /// Curvas de referencia OMS (P3, P15, P50, P85, P97) para graficar
        /// las líneas de percentil en el chart de predicciones.
        /// </summary>
        public class CurvasOms
        {
            public char   Sexo    { get; set; }
            public string Tipo    { get; set; } = string.Empty; // "Peso" | "Talla"
            public List<PuntoOms> Curvas { get; set; } = new();
        }

        public class PuntoOms
        {
            public int     EdadMeses   { get; set; }
            public decimal Percentil3  { get; set; }
            public decimal Percentil15 { get; set; }
            public decimal Percentil50 { get; set; }
            public decimal Percentil85 { get; set; }
            public decimal Percentil97 { get; set; }
        }
    }
}
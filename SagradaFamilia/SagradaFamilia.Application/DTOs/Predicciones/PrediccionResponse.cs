namespace SagradaFamilia.Application.DTOs.Predicciones
{
    public class PrediccionResponse
    {
        public bool PuedePredecir { get; set; }
        public string? Mensaje { get; set; }
        public int NinoId { get; set; }
        public List<PuntoPrediccion> Predicciones { get; set; } = new();
    }

    public class PrediccionHealth
    {
        public string Status { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string? Version { get; set; } = string.Empty;
    }
}

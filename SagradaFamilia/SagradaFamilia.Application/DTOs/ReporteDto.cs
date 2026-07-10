namespace SagradaFamilia.Application.DTOs
{
    public static class ReporteDto
    {
        public class HistoriaClinicaRequest
        {
            public int NinoId { get; set; }
            public string? Titulo { get; set; }
            public string? Usuario { get; set; }
            public string? GraficaCrecimientoBase64 { get; set; }
            public string? GraficaImcBase64 { get; set; }
        }

        public class PrediccionRequest
        {
            public int NinoId { get; set; }
            public string? Titulo { get; set; }
            public string? Usuario { get; set; }
            public string? GraficaPrediccionBase64 { get; set; }
            public string? GraficaPrecisionBase64 { get; set; }
        }
    }
}

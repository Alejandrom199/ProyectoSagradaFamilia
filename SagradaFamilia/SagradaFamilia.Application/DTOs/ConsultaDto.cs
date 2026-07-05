namespace SagradaFamilia.Application.DTOs
{
    public static class ConsultaDto
    {
        public class Response
        {
            public int Id { get; set; }
            public int CitaId { get; set; }
            public string Estado { get; set; } = string.Empty;
            public string? Motivo { get; set; }
            public string? Diagnostico { get; set; }
            public string? Indicaciones { get; set; }
            public string? Evolucion { get; set; }
            public DateTime FechaCreacion { get; set; }
        }

        public class Actualizar
        {
            public string? Motivo { get; set; }
            public string? Diagnostico { get; set; }
            public string? Indicaciones { get; set; }
            public string? Evolucion { get; set; }
        }
    }
}

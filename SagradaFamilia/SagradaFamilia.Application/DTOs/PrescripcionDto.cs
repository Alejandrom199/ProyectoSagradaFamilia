namespace SagradaFamilia.Application.DTOs
{
    public static class PrescripcionDto
    {
        public class Response
        {
            public int Id { get; set; }

            public int NinoId { get; set; }
            public string NombreNino { get; set; } = string.Empty;

            public int MedicoId { get; set; }
            public string NombreMedico { get; set; } = string.Empty;
            public string? EspecialidadMedico { get; set; }

            public string DetalleMedicamentos { get; set; } = string.Empty;
            public string? Indicaciones { get; set; }

            public DateTime FechaCreacion { get; set; }
        }

        public class Create
        {
            public int NinoId { get; set; }
            public string DetalleMedicamentos { get; set; } = string.Empty;
            public string? Indicaciones { get; set; }
        }

        public class Update
        {
            public string DetalleMedicamentos { get; set; } = string.Empty;
            public string? Indicaciones { get; set; }
        }
    }
}
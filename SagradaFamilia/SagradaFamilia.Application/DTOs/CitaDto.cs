using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.DTOs
{
    public static class CitaDto
    {
        public class Response
        {
            public int Id { get; set; }

            public int NinoId { get; set; }
            public string NombreNino { get; set; } = string.Empty;

            public int MedicoId { get; set; }
            public string NombreMedico { get; set; } = string.Empty;

            public DateTime FechaHora { get; set; }
            public string? Motivo { get; set; }

            public string Estado { get; set; } = string.Empty;

            public DateTime FechaCreacion { get; set; }
        }

        public class Create
        {
            public int NinoId { get; set; }
            public int MedicoId { get; set; }
            public DateTime FechaHora { get; set; }
            public string? Motivo { get; set; }
        }

        public class Update
        {
            public int MedicoId { get; set; }
            public DateTime FechaHora { get; set; }
            public string? Motivo { get; set; }
        }
    }
}
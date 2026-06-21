using SagradaFamilia.Domain.Enums;
using static SagradaFamilia.Application.DTOs.PrescripcionDto;

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
            public DateTime  FechaHora    { get; set; }
            public DateTime? FechaHoraFin { get; set; }
            public string? Motivo { get; set; }
            public string? NotasConsulta { get; set; }
            public string Estado { get; set; } = string.Empty;
            public DateTime FechaCreacion { get; set; }
            public bool TienePrescripcion { get; set; }
            public PrescripcionResumen? Prescripcion { get; set; }
        }

        public class Create
        {
            public int      NinoId       { get; set; }
            public int      MedicoId     { get; set; }
            public DateTime FechaHora    { get; set; }
            public DateTime FechaHoraFin { get; set; }
            public string?  Motivo       { get; set; }
        }

        public class Update
        {
            public int      MedicoId     { get; set; }
            public DateTime FechaHora    { get; set; }
            public DateTime FechaHoraFin { get; set; }
            public string?  Motivo       { get; set; }
        }
    }
}
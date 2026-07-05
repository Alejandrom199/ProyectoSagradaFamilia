namespace SagradaFamilia.Application.DTOs
{
    public static class PrescripcionDto
    {
        public class Response
        {
            public int Id { get; set; }

            public int ConsultaId { get; set; }

            // Derivado de Consulta.CitaId — se mantiene solo para no romper
            // la navegación existente del frontend (historial-prescripciones → /citas/{citaId}).
            // Prescripcion ya no tiene FK directa a Cita.
            public int CitaId { get; set; }

            public int NinoId { get; set; }
            public string NombreNino { get; set; } = string.Empty;

            public int MedicoId { get; set; }
            public string NombreMedico { get; set; } = string.Empty;
            public string? EspecialidadMedico { get; set; }

            public string? Indicaciones { get; set; }
            public List<MedicamentoDto.Response> Medicamentos { get; set; } = new();

            public DateTime FechaCreacion { get; set; }
        }

        public class Create
        {
            public int ConsultaId { get; set; }
            public List<MedicamentoDto.Item> Medicamentos { get; set; } = new();
            public string? Indicaciones { get; set; }
        }

        public class Update
        {
            public List<MedicamentoDto.Item> Medicamentos { get; set; } = new();
            public string? Indicaciones { get; set; }
        }

        public class PrescripcionResumen
        {
            public int Id { get; set; }
            public List<MedicamentoDto.Response> Medicamentos { get; set; } = new();
            public string Indicaciones { get; set; } = string.Empty;
            public DateTime FechaCreacion { get; set; }
        }
    }
}

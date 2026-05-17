namespace SagradaFamilia.Application.DTOs
{
    public static class MedicoDto
    {
        public class ListResponse
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Especialidad { get; set; }
            public string? Telefono { get; set; }
        }

        public class DetailResponse
        {
            public int Id { get; set; }
            public int UsuarioId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Especialidad { get; set; }
            public string? Telefono { get; set; }
            public DateTime FechaCreacion { get; set; }

            public List<NinoDto.ListResponse> Pacientes { get; set; } = new();
        }

        public class Create
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;

            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string? Especialidad { get; set; }
            public string? Telefono { get; set; }
        }

        public class Update
        {
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string? Especialidad { get; set; }
            public string? Telefono { get; set; }
        }
    }
}
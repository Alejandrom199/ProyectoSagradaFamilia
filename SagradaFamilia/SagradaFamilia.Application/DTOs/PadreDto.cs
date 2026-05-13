namespace SagradaFamilia.Application.DTOs
{
    public static class PadreDto
    {
        public class ListResponse
        {
            public int Id { get; set; } 
            public string NombreCompleto { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty; 
            public string? Telefono { get; set; }

            public int MedicoId { get; set; }
            public string NombreMedico { get; set; } = string.Empty;

            public int TotalHijos { get; set; }
            public bool Activo { get; set; } 
            public DateTime FechaCreacion { get; set; }
        }

        public class DetailResponse
        {
            public int Id { get; set; }
            public int UsuarioId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Telefono { get; set; }
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }

            public int MedicoId { get; set; }
            public string MedicoNombreCompleto { get; set; } = string.Empty;

            public List<NinoDto.ListResponse> Hijos { get; set; } = new();
        }

        public class Create
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;

            public int MedicoId { get; set; }

            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string? Telefono { get; set; }
        }

        public class Update
        {
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string? Telefono { get; set; }

            public int? MedicoId { get; set; }
        }
    }
}
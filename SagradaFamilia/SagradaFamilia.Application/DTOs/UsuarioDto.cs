namespace SagradaFamilia.Application.DTOs
{
    public static class UsuarioDto
    {
        public class ListResponse
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public int RolId { get; set; }
            public string RolNombre { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
            
            public bool EsMedico { get; set; } 
            public bool EsPadre { get; set; }
        }

        public class DetailResponse
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public int RolId { get; set; }
            public string RolNombre { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
            
            public int? MedicoId { get; set; }
            public int? PadreId { get; set; }
        }

        public class Create
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int RolId { get; set; }
        }

        public class Update
        {
            public int RolId { get; set; }
            public bool Activo { get; set; }
        }
    }
}
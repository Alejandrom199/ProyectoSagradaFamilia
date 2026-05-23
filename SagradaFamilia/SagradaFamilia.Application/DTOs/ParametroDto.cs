namespace SagradaFamilia.Application.DTOs
{
    public static class ParametroDto
    {
        public class Response
        {
            public int Id { get; set; }
            public string Grupo { get; set; } = string.Empty;
            public string Codigo { get; set; } = string.Empty;
            public string Valor { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
            public DateTime? FechaActualizacion { get; set; }
        }

        public class Create
        {
            public string Grupo { get; set; } = string.Empty;
            public string Codigo { get; set; } = string.Empty;
            public string Valor { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
        }

        public class Update
        {
            public string Valor { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public bool Activo { get; set; }
        }
    }
}

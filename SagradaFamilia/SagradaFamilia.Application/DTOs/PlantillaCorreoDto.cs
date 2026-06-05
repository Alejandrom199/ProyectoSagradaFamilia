namespace SagradaFamilia.Application.DTOs
{
    public static class PlantillaCorreoDto
    {
        public class Response
        {
            public int Id { get; set; }
            public string Codigo { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string Asunto { get; set; } = string.Empty;
            public string Cuerpo { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
            public DateTime? FechaActualizacion { get; set; }
        }

        public class Create
        {
            public string Codigo { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string Asunto { get; set; } = string.Empty;
            public string Cuerpo { get; set; } = string.Empty;
        }

        public class Update
        {
            public string Nombre { get; set; } = string.Empty;
            public string Asunto { get; set; } = string.Empty;
            public string Cuerpo { get; set; } = string.Empty;
            public bool Activo { get; set; }
        }
    }
}

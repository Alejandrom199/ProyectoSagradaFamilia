namespace SagradaFamilia.Application.DTOs
{
    public static class AuditoriaDto
    {
        public class Response
        {
            public int Id { get; set; }
            public int UsuarioId { get; set; }

            public string UsuarioEmail { get; set; } = string.Empty;

            public DateTime Fecha { get; set; }
            public string Accion { get; set; } = string.Empty;
            public string Tabla { get; set; } = string.Empty;
            public string ClavePrimaria { get; set; } = string.Empty;

            public string? ValoresAntiguos { get; set; }
            public string? ValoresNuevos { get; set; }

            public string? IpAddress { get; set; }
        }

        public class Create
        {
            public int UsuarioId { get; set; }
            public string Accion { get; set; } = string.Empty;
            public string Tabla { get; set; } = string.Empty;
            public string ClavePrimaria { get; set; } = string.Empty;
            public string? ValoresAntiguos { get; set; }
            public string? ValoresNuevos { get; set; }
            public string? IpAddress { get; set; }
        }

    }
}
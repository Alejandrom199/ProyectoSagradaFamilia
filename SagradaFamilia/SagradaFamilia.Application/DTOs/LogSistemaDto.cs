namespace SagradaFamilia.Application.DTOs
{
    public static class LogSistemaDto
    {
        public class Response
        {
            public int Id { get; set; }
            public DateTime FechaHora { get; set; }
            public string Nivel { get; set; } = string.Empty; 
            public string Mensaje { get; set; } = string.Empty;
            public string? Excepcion { get; set; }
            public string? Endpoint { get; set; }

            // Opcional: Para saber quién causó el error
            public int? UsuarioId { get; set; }
        }
    }
}
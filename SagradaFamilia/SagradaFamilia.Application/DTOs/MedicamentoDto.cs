namespace SagradaFamilia.Application.DTOs
{
    public static class MedicamentoDto
    {
        public class Response
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string? Presentacion { get; set; }
            public string Dosis { get; set; } = string.Empty;
            public string Frecuencia { get; set; } = string.Empty;
            public string ViaAdministracion { get; set; } = string.Empty;
            public string? Duracion { get; set; }
            public string? Cantidad { get; set; }
            public string? Observaciones { get; set; }
        }

        public class Item
        {
            public string Nombre { get; set; } = string.Empty;
            public string? Presentacion { get; set; }
            public string Dosis { get; set; } = string.Empty;
            public string Frecuencia { get; set; } = string.Empty;
            public string ViaAdministracion { get; set; } = string.Empty;
            public string? Duracion { get; set; }
            public string? Cantidad { get; set; }
            public string? Observaciones { get; set; }
        }
    }
}

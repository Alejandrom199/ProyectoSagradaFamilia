namespace SagradaFamilia.Application.DTOs
{
    public static class EventoCorreoDto
    {
        public class Response
        {
            public int      Id                { get; set; }
            public string   Codigo            { get; set; } = string.Empty;
            public string   Nombre            { get; set; } = string.Empty;
            public string   Descripcion       { get; set; } = string.Empty;
            public string[] Variables         { get; set; } = [];
            public int?     PlantillaCorreoId { get; set; }
            public string?  PlantillaNombre   { get; set; }
            public bool     PlantillaActiva   { get; set; }
        }

        public class AsignarPlantilla
        {
            public int? PlantillaCorreoId { get; set; }
        }
    }
}

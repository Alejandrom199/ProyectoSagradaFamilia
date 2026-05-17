namespace SagradaFamilia.Application.DTOs
{
    public class AlimentoDto
    {
        public class Response
        {
            public int Id { get; set; }
            public int CategoriaId { get; set; }
            public string CategoriaNombre { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public int EdadMinimaMeses { get; set; } 
            public string? Recomendacion { get; set; }
            public bool Activo { get; set; }
        }

        public class Create
        {
            public int CategoriaId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public int EdadMinimaMeses { get; set; }
            public string? Recomendacion { get; set; }
        }

        public class Update
        {
            public int CategoriaId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public int EdadMinimaMeses { get; set; }
            public string? Recomendacion { get; set; }
            public bool Activo { get; set; }
        }
    }

    public static class CategoriaDto
    {
        public class Response
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public int TotalAlimentos { get; set; }
            public bool Activo { get; set; }
        }

        public class Create
        {
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
        }

        public class Update
        {
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public bool Activo { get; set; }
        }
    }
}
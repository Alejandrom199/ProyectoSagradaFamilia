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

        public class ImportResultado
        {
            public int TotalProcesadas { get; set; }
            public int Importados { get; set; }
            public int Actualizados { get; set; }
            public List<ImportError> Errores { get; set; } = [];
        }

        public class ImportError
        {
            public int Fila { get; set; }
            public string Mensaje { get; set; } = string.Empty;
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
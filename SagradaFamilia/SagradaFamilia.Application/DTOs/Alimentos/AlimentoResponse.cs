namespace SagradaFamilia.Application.DTOs.Alimentos
{
    public class AlimentoResponse
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int EdadMinimaIntro { get; set; }
        public int? EdadMaxima { get; set; }
        public string? Recomendacion { get; set; }
    }
}

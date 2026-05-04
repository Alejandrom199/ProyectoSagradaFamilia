namespace SagradaFamilia.Application.DTOs.Alimentos
{
    public class CategoriaResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int TotalAlimentos { get; set; }
    }
}

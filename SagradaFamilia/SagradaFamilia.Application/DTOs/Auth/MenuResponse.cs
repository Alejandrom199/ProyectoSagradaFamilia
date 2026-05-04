namespace SagradaFamilia.Application.DTOs.Auth
{
    public class MenuResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Icono { get; set; }
        public int Orden { get; set; }
        public List<OpcionResponse> Opciones { get; set; } = new();
    }
}

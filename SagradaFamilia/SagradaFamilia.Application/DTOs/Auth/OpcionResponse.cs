namespace SagradaFamilia.Application.DTOs.Auth
{
    public class OpcionResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ruta { get; set; }
        public string? Icono { get; set; }
        public int Orden { get; set; }
        public List<string> Acciones { get; set; } = new();
    }
}

namespace SagradaFamilia.Application.DTOs.Ninos
{
    public class NinoResponse
    {
        public int Id { get; set; }
        public int RepresentanteId { get; set; }
        public string NombrePadre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public char Sexo { get; set; }
        public int EdadMeses { get; set; }   // calculado
        public DateTime FechaCreacion { get; set; }
    }
}

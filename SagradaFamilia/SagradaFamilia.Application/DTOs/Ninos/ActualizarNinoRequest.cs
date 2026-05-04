namespace SagradaFamilia.Application.DTOs.Ninos
{
    public class ActualizarNinoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public char Sexo { get; set; }
    }
}

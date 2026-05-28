namespace SagradaFamilia.Application.DTOs
{
    public static class MedidaDto
    {
        public class Response
        {
            public int Id { get; set; }
            public int NinoId { get; set; }
            public string NombreNino { get; set; } = string.Empty;

            public int MedicoId { get; set; }
            public string NombreMedico { get; set; } = string.Empty;

            public DateOnly FechaMedicion { get; set; }
            public decimal Peso { get; set; }
            public decimal Talla { get; set; }

            public string EstadoNutricional { get; set; } = string.Empty;
            public decimal PercentilPeso { get; set; }
            public decimal PercentilTalla { get; set; }

            public DateTime FechaRegistro { get; set; }
        }

        public class Create
        {
            public int NinoId { get; set; }
            public DateOnly FechaMedicion { get; set; }
            public decimal Peso { get; set; }
            public decimal Talla { get; set; }
        }

        public class Update
        {
            public DateOnly FechaMedicion { get; set; }
            public decimal Peso { get; set; }
            public decimal Talla { get; set; }
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
}
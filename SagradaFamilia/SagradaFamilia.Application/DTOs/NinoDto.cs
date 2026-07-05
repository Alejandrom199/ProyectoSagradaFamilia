namespace SagradaFamilia.Application.DTOs
{
    public static class NinoDto
    {
        public class ListResponse
        {
            public int Id { get; set; }
            public int PadreId { get; set; }
            public string NombrePadre { get; set; } = string.Empty;

            public int MedicoId { get; set; }
            public string NombreMedico { get; set; } = string.Empty;

            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public DateOnly FechaNacimiento { get; set; }
            public char Sexo { get; set; }
            public int EdadMeses { get; set; }
            public DateTime FechaCreacion { get; set; }
        }

        public class DetailResponse
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public DateOnly FechaNacimiento { get; set; }
            public char Sexo { get; set; }
            public int EdadMeses { get; set; }
            public DateTime FechaCreacion { get; set; }

            public int PadreId { get; set; }
            public string PadreNombreCompleto { get; set; } = string.Empty;
            public string PadreEmail { get; set; } = string.Empty;
            public string PadreTelefono { get; set; } = string.Empty;

            public int MedicoId { get; set; }
            public string MedicoNombreCompleto { get; set; } = string.Empty;
            public string? MedicoEspecialidad { get; set; }
        }

        public class Create
        {
            public int PadreId { get; set; }
            public int MedicoId { get; set; }

            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public DateOnly FechaNacimiento { get; set; }
            public char Sexo { get; set; }
        }

        public class Update
        {
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public DateOnly FechaNacimiento { get; set; }
            public char Sexo { get; set; }
        }

        public class ChangeMedico
        {
            public int MedicoId { get; set; }
        }

        public class ChangePadre
        {
            public int PadreId { get; set; }
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
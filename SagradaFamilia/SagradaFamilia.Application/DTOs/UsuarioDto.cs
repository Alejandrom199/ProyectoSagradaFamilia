namespace SagradaFamilia.Application.DTOs
{
    public static class UsuarioDto
    {
        public class ListResponse
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public int RolId { get; set; }
            public string RolNombre { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
            
            public bool EsMedico { get; set; }
            public bool EsPadre { get; set; }

            public int? MedicoId { get; set; }
            public int? PadreId { get; set; }
        }

        public class DetailResponse
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public int RolId { get; set; }
            public string RolNombre { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
            
            public int? MedicoId { get; set; }
            public int? PadreId { get; set; }
        }

        public class Create
        {
            public string Email { get; set; } = string.Empty;
            public int RolId { get; set; }
        }

        public class Update
        {
            public int RolId { get; set; }
            public bool Activo { get; set; }
        }

        public class ActualizarEstadoRequest
        {
            public bool Activo { get; set; }
            public string? Motivo { get; set; }
        }

        public class EstadoHistorialResponse
        {
            public int Id { get; set; }
            public bool EstadoNuevo { get; set; }
            public string? Motivo { get; set; }
            public DateTime FechaCambio { get; set; }
            public string RealizadoPor { get; set; } = string.Empty;
        }

        public class ImportResultado
        {
            public int TotalProcesadas { get; set; }
            public int Importados { get; set; }
            public int Actualizados { get; set; }
            public List<ImportError> Errores { get; set; } = [];
            public List<ImportDetalle> Detalle { get; set; } = [];
        }

        public class ImportError
        {
            public int Fila { get; set; }
            public string Mensaje { get; set; } = string.Empty;
        }

        public class ImportDetalle
        {
            public int Fila { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string Rol { get; set; } = string.Empty;
            public string Accion { get; set; } = string.Empty;
        }
    }
}
namespace SagradaFamilia.Application.DTOs.Auth
{
    public static class PermisoDto
    {
        public class AsignarRequest
        {
            public int UsuarioId { get; set; }
            public int OpcionAccionId { get; set; }
            public bool Permitido { get; set; } = true;
        }

        public class ModuloPermisoResponse
        {
            public int ModuloId { get; set; }
            public string ModuloNombre { get; set; } = string.Empty;
            public List<OpcionPermisoResponse> Opciones { get; set; } = [];
        }

        public class OpcionPermisoResponse
        {
            public int OpcionId { get; set; }
            public string OpcionNombre { get; set; } = string.Empty;
            public List<AccionPermisoResponse> Acciones { get; set; } = [];
        }

        public class AccionPermisoResponse
        {
            public int OpcionAccionId { get; set; }
            public string AccionNombre { get; set; } = string.Empty;
            public bool Permitido { get; set; }
        }

        public class ActualizarPermisoRolRequest
        {
            public int OpcionAccionId { get; set; }
            public bool Permitido { get; set; }
        }
    }
}
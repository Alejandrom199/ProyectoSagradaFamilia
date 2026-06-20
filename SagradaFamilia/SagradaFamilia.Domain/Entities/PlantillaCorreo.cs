using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class PlantillaCorreo : AuditableEntity
    {
        /// <summary>
        /// Código técnico heredado (usado por plantillas del sistema).
        /// Las plantillas creadas por el administrador tienen Codigo = null.
        /// La asignación a eventos se gestiona a través de EventoCorreo.
        /// </summary>
        public string? Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Cuerpo { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}

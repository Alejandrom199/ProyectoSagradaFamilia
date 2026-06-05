using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class PlantillaCorreo : AuditableEntity
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Cuerpo { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}

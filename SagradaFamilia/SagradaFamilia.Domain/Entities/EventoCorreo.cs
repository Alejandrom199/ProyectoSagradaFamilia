using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    /// <summary>
    /// Representa un momento concreto del sistema que dispara un correo electrónico.
    /// Los eventos son fijos (definidos en código), pero la plantilla asignada es configurable
    /// desde el panel de administración sin necesidad de tocar el código fuente.
    /// </summary>
    public class EventoCorreo : BaseEntity
    {
        public string  Codigo             { get; set; } = string.Empty;
        public string  Nombre             { get; set; } = string.Empty;
        public string  Descripcion        { get; set; } = string.Empty;

        /// <summary>JSON array de nombres de variable: ["NOMBRE","APELLIDO","LINK"]</summary>
        public string  Variables          { get; set; } = "[]";

        public int?    PlantillaCorreoId  { get; set; }
        public PlantillaCorreo? Plantilla { get; set; }
    }
}

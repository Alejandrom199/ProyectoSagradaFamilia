using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class ParametroSistema : AuditableEntity
    {
        public string Grupo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}

using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Entities
{
    public class OmsReferencia : BaseEntity
    {
        public char Sexo { get; set; }
        public int EdadMeses { get; set; }
        public TipoReferencia Tipo { get; set; }

        // Percentiles
        public decimal Percentil3 { get; set; }
        public decimal Percentil15 { get; set; }
        public decimal Percentil50 { get; set; } // La media
        public decimal Percentil85 { get; set; }
        public decimal Percentil97 { get; set; }

    }
}

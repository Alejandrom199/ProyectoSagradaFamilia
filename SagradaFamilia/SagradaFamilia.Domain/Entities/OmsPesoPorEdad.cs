using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Domain.Entities
{
    public class OmsPesoPorEdad : BaseEntity
    {
        public char Sexo { get; set; }
        public int EdadMeses { get; set; }
        public decimal Percentil3 { get; set; }
        public decimal Percentil15 { get; set; }
        public decimal Percentil50 { get; set; }
        public decimal Percentil85 { get; set; }
        public decimal Percentil97 { get; set; }
    }
}

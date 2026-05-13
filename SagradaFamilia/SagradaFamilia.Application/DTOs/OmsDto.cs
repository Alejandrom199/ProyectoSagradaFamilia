namespace SagradaFamilia.Application.DTOs
{
    public static class OmsDto
    {
        public class Response
        {
            public int Id { get; set; }
            public char Sexo { get; set; }
            public int EdadMeses { get; set; }

            public string TipoReferencia { get; set; } = string.Empty;

            public decimal Percentil3 { get; set; }
            public decimal Percentil15 { get; set; }
            public decimal Percentil50 { get; set; } // La mediana/ideal
            public decimal Percentil85 { get; set; }
            public decimal Percentil97 { get; set; }
        }
    }
}
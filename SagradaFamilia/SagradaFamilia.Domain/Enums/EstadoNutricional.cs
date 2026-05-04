namespace SagradaFamilia.Domain.Enums
{
    public enum EstadoNutricional
    {
        BajoPesoSevero,   // por debajo de P3
        BajoPeso,         // entre P3 y P15
        Normal,           // entre P15 y P85
        Sobrepeso,        // entre P85 y P97
        Obesidad          // por encima de P97
    }
}

namespace SagradaFamilia.Domain.Enums
{
    public enum EstadoPrediccion
    {
        SinDatosSuficientes,   // menos de 3 medidas
        Disponible,            // puede predecir
        ErrorServicio          // Python no responde
    }
}

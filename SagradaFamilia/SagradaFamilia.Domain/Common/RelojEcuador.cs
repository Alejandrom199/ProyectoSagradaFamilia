namespace SagradaFamilia.Domain.Common
{
    // El sistema almacena y compara FechaHora como hora local de Ecuador (sin conversión UTC).
    // En producción el servidor corre en UTC, por lo que DateTime.Now no representa la hora
    // local de Ecuador: hay que calcularla explícitamente a partir de DateTime.UtcNow.
    // Ecuador (America/Guayaquil) es UTC-5 fijo, sin horario de verano.
    public static class RelojEcuador
    {
        private const int OffsetHoras = -5;

        public static DateTime Ahora => DateTime.UtcNow.AddHours(OffsetHoras);

        public static DateTime Hoy => Ahora.Date;
    }
}

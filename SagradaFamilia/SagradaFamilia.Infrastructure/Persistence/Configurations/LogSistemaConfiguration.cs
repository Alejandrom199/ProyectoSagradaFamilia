using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class LogSistemaConfiguration : IEntityTypeConfiguration<LogSistema>
    {
        public void Configure(EntityTypeBuilder<LogSistema> builder)
        {
            builder.ToTable("LogsSistema");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Nivel)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(l => l.Mensaje)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(l => l.Excepcion)
                .HasMaxLength(4000);

            builder.Property(l => l.Endpoint)
                .HasMaxLength(500);

            builder.Property(l => l.IpAddress)
                .HasMaxLength(50);

            builder.Property(l => l.FechaHora)
                .HasDefaultValueSql("GETUTCDATE()");

            // Índice para búsquedas por fecha y nivel
            builder.HasIndex(l => l.FechaHora);
            builder.HasIndex(l => l.Nivel);
        }
    }
}

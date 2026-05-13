using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class PrediccionConfiguration : IEntityTypeConfiguration<Prediccion>
    {
        public void Configure(EntityTypeBuilder<Prediccion> builder)
        {
            builder.ToTable("Predicciones");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FechaCalculo)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.FechaObjetivo)
                .IsRequired();

            builder.Property(p => p.ProyeccionMeses)
                .IsRequired();

            builder.Property(p => p.Tipo)
                .IsRequired();

            builder.Property(p => p.ValorPredicho)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.ValorMinimo)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.ValorMaximo)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.ValorReal)
                .IsRequired(false)
                .HasColumnType("decimal(5,2)");

            // Relación
            builder.HasOne(p => p.Nino)
                .WithMany(n => n.Predicciones)
                .HasForeignKey(p => p.NinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
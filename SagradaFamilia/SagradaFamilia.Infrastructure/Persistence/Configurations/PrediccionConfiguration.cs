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

            builder.Property(p => p.PesoPredicho)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.PesoMinimo)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.PesoMaximo)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.PesoReal)
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.Meses)
                .IsRequired();

            builder.Property(p => p.FechaCalculo)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.Nino)
                .WithMany(n => n.Predicciones)
                .HasForeignKey(p => p.NinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class ReporteGeneradoConfiguration : IEntityTypeConfiguration<ReporteGenerado>
    {
        public void Configure(EntityTypeBuilder<ReporteGenerado> builder)
        {
            builder.ToTable("ReportesGenerados");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Uid)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(r => r.Tipo)
                .IsRequired();

            builder.Property(r => r.FechaGeneracion)
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(r => r.Uid)
                .IsUnique();

            builder.HasIndex(r => new { r.Tipo, r.FechaGeneracion });

            // Relaciones
            builder.HasOne(r => r.Nino)
                .WithMany()
                .HasForeignKey(r => r.NinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.UsuarioGenerador)
                .WithMany()
                .HasForeignKey(r => r.UsuarioGeneradorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

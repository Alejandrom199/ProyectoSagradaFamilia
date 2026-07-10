using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class UsuarioEstadoHistorialConfiguration : IEntityTypeConfiguration<UsuarioEstadoHistorial>
    {
        public void Configure(EntityTypeBuilder<UsuarioEstadoHistorial> builder)
        {
            builder.ToTable("UsuarioEstadoHistorial");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Motivo)
                .HasMaxLength(500);

            builder.Property(h => h.FechaCambio)
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(h => h.UsuarioId);

            // Relaciones
            builder.HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.UsuarioQueRealizoCambio)
                .WithMany()
                .HasForeignKey(h => h.UsuarioQueRealizoCambioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

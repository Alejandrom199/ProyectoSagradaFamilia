using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class MedidaConfiguration : AuditableEntityConfiguration<Medida>
    {
        public override void Configure(EntityTypeBuilder<Medida> builder)
        {
            base.Configure(builder);

            builder.ToTable("Medidas");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Peso)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Talla)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.FechaMedicion)
                .IsRequired();

            builder.Property(m => m.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(m => new { m.NinoId, m.FechaMedicion })
                .IsUnique()
                .HasFilter("[Eliminado] = 0");

            // Relaciones
            builder.HasOne(m => m.Nino)
                .WithMany(n => n.Medidas)
                .HasForeignKey(m => m.NinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Medico)
                .WithMany(med => med.Medidas)
                .HasForeignKey(m => m.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
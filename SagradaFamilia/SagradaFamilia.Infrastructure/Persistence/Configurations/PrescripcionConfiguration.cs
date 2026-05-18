using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class PrescripcionConfiguration : AuditableEntityConfiguration<Prescripcion>
    {
        public override void Configure(EntityTypeBuilder<Prescripcion> builder)
        {
            base.Configure(builder);

            builder.ToTable("Prescripciones");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.DetalleMedicamentos)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.Indicaciones)
                .HasMaxLength(2000);

            builder.Property(p => p.Diagnostico)
                .HasMaxLength(500);

            // Relaciones
            builder.HasOne(p => p.Nino)
                .WithMany()
                .HasForeignKey(p => p.NinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Medico)
                .WithMany()
                .HasForeignKey(p => p.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Cita) 
                .WithMany(c => c.Prescripciones)
                .HasForeignKey(p => p.CitaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class ConsultaConfiguration : AuditableEntityConfiguration<Consulta>
    {
        public override void Configure(EntityTypeBuilder<Consulta> builder)
        {
            base.Configure(builder);

            builder.ToTable("Consultas");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Motivo)
                .HasMaxLength(500);

            builder.Property(c => c.Diagnostico)
                .HasMaxLength(500);

            builder.Property(c => c.Indicaciones)
                .HasMaxLength(2000);

            builder.Property(c => c.Evolucion)
                .HasMaxLength(2000);

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasDefaultValue(EstadoConsulta.EnCurso);

            // Relaciones
            builder.HasOne(c => c.Cita)
                .WithOne(cita => cita.Consulta)
                .HasForeignKey<Consulta>(c => c.CitaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.CitaId)
                .IsUnique();

            builder.HasOne(c => c.Nino)
                .WithMany()
                .HasForeignKey(c => c.NinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Medico)
                .WithMany()
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

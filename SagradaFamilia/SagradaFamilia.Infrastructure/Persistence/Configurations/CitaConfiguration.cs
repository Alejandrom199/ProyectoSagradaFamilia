using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class CitaConfiguration : AuditableEntityConfiguration<Cita>
    {
        public override void Configure(EntityTypeBuilder<Cita> builder)
        {
            base.Configure(builder);

            builder.ToTable("Citas");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FechaHora)
                .IsRequired();

            builder.Property(c => c.Motivo)
                .HasMaxLength(500);

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasDefaultValue(Domain.Enums.EstadoCita.Pendiente);

            // Relaciones
            builder.HasOne(c => c.Nino)
                .WithMany(n => n.Citas)
                .HasForeignKey(c => c.NinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Medico)
                .WithMany() // Un médico gestiona sus citas a través de consultas
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice para evitar que un médico tenga dos citas al mismo tiempo
            builder.HasIndex(c => new { c.MedicoId, c.FechaHora })
                .IsUnique()
                .HasFilter("[Estado] != 2 AND [Eliminado] = 0"); // 2 asumiendo que es 'Cancelada'
        }
    }
}
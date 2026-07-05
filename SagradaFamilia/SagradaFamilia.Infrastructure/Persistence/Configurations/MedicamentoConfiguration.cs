using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class MedicamentoConfiguration : AuditableEntityConfiguration<Medicamento>
    {
        public override void Configure(EntityTypeBuilder<Medicamento> builder)
        {
            base.Configure(builder);

            builder.ToTable("Medicamentos");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Presentacion)
                .HasMaxLength(200);

            builder.Property(m => m.Dosis)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Frecuencia)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.ViaAdministracion)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Duracion)
                .HasMaxLength(100);

            builder.Property(m => m.Cantidad)
                .HasMaxLength(100);

            builder.Property(m => m.Observaciones)
                .HasMaxLength(500);

            builder.HasOne(m => m.Prescripcion)
                .WithMany(p => p.Medicamentos)
                .HasForeignKey(m => m.PrescripcionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

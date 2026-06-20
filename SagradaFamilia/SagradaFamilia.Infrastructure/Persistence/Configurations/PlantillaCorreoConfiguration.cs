using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class PlantillaCorreoConfiguration : AuditableEntityConfiguration<PlantillaCorreo>
    {
        public override void Configure(EntityTypeBuilder<PlantillaCorreo> builder)
        {
            base.Configure(builder);

            builder.ToTable("PlantillasCorreo");

            builder.HasKey(p => p.Id);

            // Codigo es nullable: las plantillas del sistema tienen código; las del admin no.
            builder.Property(p => p.Codigo)
                .IsRequired(false)
                .HasMaxLength(50);

            // Índice único parcial: solo cuando hay código y la plantilla no fue eliminada.
            // PostgreSQL permite múltiples NULLs aunque haya índice único.
            builder.HasIndex(p => p.Codigo)
                .IsUnique()
                .HasFilter("\"Eliminado\" = false AND \"Codigo\" IS NOT NULL");

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Asunto)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Cuerpo)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(p => p.Activo)
                .HasDefaultValue(true);
        }
    }
}

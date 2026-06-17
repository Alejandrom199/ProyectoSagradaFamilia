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

            builder.Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

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

            builder.HasIndex(p => p.Codigo)
                .IsUnique()
                .HasFilter("\"Eliminado\" = false");
        }
    }
}

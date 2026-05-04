using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class AlimentoConfiguration : AuditableEntityConfiguration<Alimento>
    {
        public override void Configure(EntityTypeBuilder<Alimento> builder)
        {
            base.Configure(builder);

            builder.ToTable("Alimentos");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Descripcion)
                .HasMaxLength(500);

            builder.Property(a => a.EdadMinimaIntro)
                .IsRequired();

            builder.Property(a => a.Recomendacion)
                .HasMaxLength(500);

            builder.Property(a => a.Activo)
                .HasDefaultValue(true);
        }
    }
}

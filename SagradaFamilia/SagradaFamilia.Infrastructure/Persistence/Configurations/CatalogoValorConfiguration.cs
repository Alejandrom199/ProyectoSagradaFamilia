using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class CatalogoValorConfiguration : AuditableEntityConfiguration<CatalogoValor>
    {
        public override void Configure(EntityTypeBuilder<CatalogoValor> builder)
        {
            base.Configure(builder);

            builder.ToTable("CatalogosValor");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Tipo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Valor)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Descripcion)
                .HasMaxLength(500);

            builder.Property(c => c.Activo)
                .HasDefaultValue(true);

            builder.HasIndex(c => new { c.Tipo, c.Codigo })
                .IsUnique()
                .HasFilter("\"Eliminado\" = false");
        }
    }
}

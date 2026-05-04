using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class CategoriaAlimentoConfiguration : AuditableEntityConfiguration<CategoriaAlimento>
    {
        public override void Configure(EntityTypeBuilder<CategoriaAlimento> builder)
        {
            base.Configure(builder);

            builder.ToTable("CategoriasAlimentos");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(c => c.Descripcion)
                .HasMaxLength(300);

            builder.Property(c => c.Activo)
                .HasDefaultValue(true);

            builder.HasMany(c => c.Alimentos)
                .WithOne(a => a.Categoria)
                .HasForeignKey(a => a.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

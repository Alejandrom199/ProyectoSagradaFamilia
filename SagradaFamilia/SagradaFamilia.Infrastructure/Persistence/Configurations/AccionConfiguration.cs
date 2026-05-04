using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class AccionConfiguration : IEntityTypeConfiguration<Accion>
    {
        public void Configure(EntityTypeBuilder<Accion> builder)
        {
            builder.ToTable("Acciones");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(a => a.Nombre)
                .IsUnique();

            builder.HasMany(a => a.OpcionAcciones)
                .WithOne(oa => oa.Accion)
                .HasForeignKey(oa => oa.AccionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class OpcionAccionConfiguration : IEntityTypeConfiguration<OpcionAccion>
    {
        public void Configure(EntityTypeBuilder<OpcionAccion> builder)
        {
            builder.ToTable("OpcionAcciones");

            builder.HasKey(oa => oa.Id);

            // Índice único — no puede haber dos veces la misma acción en la misma opción
            builder.HasIndex(oa => new { oa.OpcionId, oa.AccionId })
                .IsUnique();

            builder.HasOne(oa => oa.Accion)
                .WithMany(a => a.OpcionAcciones)
                .HasForeignKey(oa => oa.AccionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(oa => oa.RolPermisos)
                .WithOne(rp => rp.OpcionAccion)
                .HasForeignKey(rp => rp.OpcionAccionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(oa => oa.UsuarioPermisos)
                .WithOne(up => up.OpcionAccion)
                .HasForeignKey(up => up.OpcionAccionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

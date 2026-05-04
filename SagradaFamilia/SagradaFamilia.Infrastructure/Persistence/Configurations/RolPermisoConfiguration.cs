using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
    {
        public void Configure(EntityTypeBuilder<RolPermiso> builder)
        {
            builder.ToTable("RolPermisos");

            builder.HasKey(rp => rp.Id);

            builder.Property(rp => rp.Permitido)
                .HasDefaultValue(true);

            // Índice único — un rol no puede tener dos veces el mismo OpcionAccion
            builder.HasIndex(rp => new { rp.RolId, rp.OpcionAccionId })
                .IsUnique();

            builder.HasOne(rp => rp.Rol)
                .WithMany(r => r.RolPermisos)
                .HasForeignKey(rp => rp.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rp => rp.OpcionAccion)
                .WithMany(oa => oa.RolPermisos)
                .HasForeignKey(rp => rp.OpcionAccionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

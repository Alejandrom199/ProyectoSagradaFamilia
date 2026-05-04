using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class UsuarioPermisoConfiguration : IEntityTypeConfiguration<UsuarioPermiso>
    {
        public void Configure(EntityTypeBuilder<UsuarioPermiso> builder)
        {
            builder.ToTable("UsuarioPermisos");

            builder.HasKey(up => up.Id);

            builder.Property(up => up.Permitido)
                .HasDefaultValue(true);

            // Índice único — un usuario no puede tener dos veces el mismo OpcionAccion
            builder.HasIndex(up => new { up.UsuarioId, up.OpcionAccionId })
                .IsUnique();
        }
    }
}

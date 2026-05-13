using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : AuditableEntityConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder);

            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Activo)
                .HasDefaultValue(true);

            // Relaciones

            // Un Usuario pertenece a un Rol
            builder.HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un Usuario puede tener muchos RefreshTokens para manejar sesiones en varios dispositivos
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(r => r.Usuario)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un Usuario puede tener permisos específicos
            builder.HasMany(u => u.Permisos)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
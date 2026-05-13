using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class PadreConfiguration : AuditableEntityConfiguration<Padre>
    {
        public override void Configure(EntityTypeBuilder<Padre> builder)
        {
            base.Configure(builder);

            builder.ToTable("Padres");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Telefono)
                .HasMaxLength(15);

            // Relaciones

            builder.HasOne(p => p.Usuario)
                .WithOne(u => u.Padre)
                .HasForeignKey<Padre>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Medico)
                .WithMany(m => m.Padres)
                .HasForeignKey(p => p.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Ninos)
                .WithOne(n => n.Padre)
                .HasForeignKey(n => n.PadreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
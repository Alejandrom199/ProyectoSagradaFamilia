using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class MedicoConfiguration : AuditableEntityConfiguration<Medico>
    {
        public override void Configure(EntityTypeBuilder<Medico> builder)
        {
            base.Configure(builder);

            builder.ToTable("Medicos");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Especialidad)
                .HasMaxLength(100);

            builder.Property(m => m.Telefono)
                .HasMaxLength(15);

            builder.Property(m => m.FirmaImagen)
                .HasColumnType("text");

            // Relaciones

            builder.HasOne(m => m.Usuario)
                .WithOne(u => u.Medico)
                .HasForeignKey<Medico>(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Padres)
                .WithOne(p => p.Medico)
                .HasForeignKey(p => p.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Ninos)
                .WithOne(n => n.Medico)
                .HasForeignKey(n => n.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Medidas)
                .WithOne(medida => medida.Medico)
                .HasForeignKey(medida => medida.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.ToTable("Auditorias");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Accion)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Tabla)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.ClavePrimaria)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.IpAddress)
                .HasMaxLength(50);

            builder.Property(a => a.Fecha)
                .HasDefaultValueSql("NOW()");

            builder.Property(a => a.ValoresAntiguos)
                .HasColumnType("text");

            builder.Property(a => a.ValoresNuevos)
                .HasColumnType("text");

            // Relación con el Usuario que realizó el cambio
            builder.HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices de búsqueda
            builder.HasIndex(a => a.Fecha);
            builder.HasIndex(a => a.Tabla);
        }
    }
}
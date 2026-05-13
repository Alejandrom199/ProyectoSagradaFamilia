using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class OpcionConfiguration : AuditableEntityConfiguration<Opcion>
    {
        public override void Configure(EntityTypeBuilder<Opcion> builder)
        {
            base.Configure(builder);

            builder.ToTable("Opciones");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Ruta)
                .HasMaxLength(150);

            builder.Property(o => o.Icono)
                .HasMaxLength(50);

            builder.Property(o => o.Orden)
                .IsRequired();

            builder.Property(o => o.Activo)
                .HasDefaultValue(true);

            // Relación Uno a Muchos con OpcionAcciones (Crear, Editar, etc.)
            builder.HasMany(o => o.OpcionAcciones)
                .WithOne(oa => oa.Opcion)
                .HasForeignKey(oa => oa.OpcionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
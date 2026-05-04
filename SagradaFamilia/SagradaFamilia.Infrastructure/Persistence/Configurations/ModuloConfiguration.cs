using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class ModuloConfiguration : AuditableEntityConfiguration<Modulo>
    {
        public override void Configure(EntityTypeBuilder<Modulo> builder)
        {
            base.Configure(builder);

            builder.ToTable("Modulos");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Descripcion)
                .HasMaxLength(300);

            builder.Property(m => m.Icono)
                .HasMaxLength(50);

            builder.Property(m => m.Orden)
                .IsRequired();

            builder.Property(m => m.Activo)
                .HasDefaultValue(true);

            builder.HasMany(m => m.Opciones)
                .WithOne(o => o.Modulo)
                .HasForeignKey(o => o.ModuloId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

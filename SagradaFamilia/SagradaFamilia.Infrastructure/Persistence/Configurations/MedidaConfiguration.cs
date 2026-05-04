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
    public class MedidaConfiguration : AuditableEntityConfiguration<Medida>
    {
        public override void Configure(EntityTypeBuilder<Medida> builder)
        {
            base.Configure(builder);

            builder.ToTable("Medidas");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Peso)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Talla)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.FechaMedicion)
                .IsRequired();

            builder.Property(m => m.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");

            // Máximo una medida por niño por mes
            builder.HasIndex(m => new { m.NinoId, m.FechaMedicion })
                .IsUnique()
                .HasFilter("[Eliminado] = 0");
        }
    }
}

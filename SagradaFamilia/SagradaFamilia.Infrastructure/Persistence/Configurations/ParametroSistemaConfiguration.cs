using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class ParametroSistemaConfiguration : AuditableEntityConfiguration<ParametroSistema>
    {
        public override void Configure(EntityTypeBuilder<ParametroSistema> builder)
        {
            base.Configure(builder);

            builder.ToTable("ParametrosSistema");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Grupo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Valor)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Descripcion)
                .HasMaxLength(500);

            builder.Property(p => p.Activo)
                .HasDefaultValue(true);

            builder.HasIndex(p => new { p.Grupo, p.Codigo })
                .IsUnique()
                .HasFilter("[Eliminado] = 0");
        }
    }
}

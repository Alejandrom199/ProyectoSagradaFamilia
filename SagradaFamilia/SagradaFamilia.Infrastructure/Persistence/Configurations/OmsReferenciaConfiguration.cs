using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class OmsReferenciaConfiguration : IEntityTypeConfiguration<OmsReferencia>
    {
        public void Configure(EntityTypeBuilder<OmsReferencia> builder)
        {
            builder.ToTable("OMS_Referencias");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Sexo)
                .IsRequired()
                .HasColumnType("char(1)");

            builder.Property(o => o.EdadMeses)
                .IsRequired();

            builder.Property(o => o.Tipo)
                .IsRequired();

            builder.Property(o => o.Percentil3).HasColumnType("decimal(5,2)");
            builder.Property(o => o.Percentil15).HasColumnType("decimal(5,2)");
            builder.Property(o => o.Percentil50).HasColumnType("decimal(5,2)");
            builder.Property(o => o.Percentil85).HasColumnType("decimal(5,2)");
            builder.Property(o => o.Percentil97).HasColumnType("decimal(5,2)");

            builder.HasIndex(o => new { o.Sexo, o.EdadMeses, o.Tipo })
                .IsUnique();
        }
    }
}
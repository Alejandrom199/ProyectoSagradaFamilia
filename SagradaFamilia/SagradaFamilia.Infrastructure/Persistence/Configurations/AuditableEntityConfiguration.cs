using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Common;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : AuditableEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.FechaActualizacion)
                .IsRequired(false);

            builder.Property(e => e.Eliminado)
                .HasDefaultValue(false);

            builder.Property(e => e.FechaEliminacion)
                .IsRequired(false);
        }
    }
}

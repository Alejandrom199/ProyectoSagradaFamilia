using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(r => r.FechaExpiracion)
                .IsRequired();

            builder.Property(r => r.Revocado)
                .HasDefaultValue(false);

            builder.Property(r => r.FechaCreacion)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}

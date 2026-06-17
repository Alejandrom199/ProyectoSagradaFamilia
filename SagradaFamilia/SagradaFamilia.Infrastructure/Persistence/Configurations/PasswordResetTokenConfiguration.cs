using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("PasswordResetTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.FechaExpiracion)
                .IsRequired();

            builder.Property(t => t.Usado)
                .HasDefaultValue(false);

            builder.Property(t => t.FechaUso)
                .IsRequired(false);

            builder.Property(t => t.FechaCreacion)
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(t => t.Token)
                .IsUnique();

            builder.HasOne(t => t.Usuario)
                .WithMany()
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

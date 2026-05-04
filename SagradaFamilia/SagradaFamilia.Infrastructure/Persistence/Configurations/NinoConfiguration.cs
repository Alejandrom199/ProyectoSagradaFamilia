using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class NinoConfiguration : AuditableEntityConfiguration<Nino>
    {
        public override void Configure(EntityTypeBuilder<Nino> builder)
        {
            base.Configure(builder);

            builder.ToTable("Ninos");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.FechaNacimiento)
                .IsRequired();

            builder.Property(n => n.Sexo)
                .IsRequired()
                .HasColumnType("char(1)");

            builder.HasMany(n => n.Medidas)
                .WithOne(m => m.Nino)
                .HasForeignKey(m => m.NinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(n => n.Predicciones)
                .WithOne(p => p.Nino)
                .HasForeignKey(p => p.NinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

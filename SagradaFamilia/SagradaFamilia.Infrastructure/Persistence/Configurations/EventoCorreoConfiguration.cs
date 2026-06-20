using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Configurations
{
    public class EventoCorreoConfiguration : IEntityTypeConfiguration<EventoCorreo>
    {
        public void Configure(EntityTypeBuilder<EventoCorreo> builder)
        {
            builder.ToTable("EventosCorreo");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.Codigo)
                .IsUnique();

            builder.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Variables)
                .IsRequired()
                .HasColumnType("text")
                .HasDefaultValue("[]");

            builder.HasOne(e => e.Plantilla)
                .WithMany()
                .HasForeignKey(e => e.PlantillaCorreoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

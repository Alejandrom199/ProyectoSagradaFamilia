using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class CatalogoValorSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.CatalogosValor.AnyAsync())
                return;

            var valores = new List<CatalogoValor>
            {
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "PEDIATRIA_GENERAL", Valor = "Pediatría General", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "NEONATOLOGIA", Valor = "Neonatología", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "NUTRICION_PEDIATRICA", Valor = "Nutrición Pediátrica", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "ENDOCRINOLOGIA_PEDIATRICA", Valor = "Endocrinología Pediátrica", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "GASTROENTEROLOGIA_PEDIATRICA", Valor = "Gastroenterología Pediátrica", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "NEUROLOGIA_PEDIATRICA", Valor = "Neurología Pediátrica", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "CARDIOLOGIA_PEDIATRICA", Valor = "Cardiología Pediátrica", Activo = true },
                new() { Tipo = "ESPECIALIDAD_MEDICA", Codigo = "ALERGOLOGIA_PEDIATRICA", Valor = "Alergología Pediátrica", Activo = true },
            };

            context.CatalogosValor.AddRange(valores);
            await context.SaveChangesAsync();
        }
    }
}

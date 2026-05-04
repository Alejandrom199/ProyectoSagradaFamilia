using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class AlimentoSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.CategoriasAlimentos.AnyAsync())
                return;

            var categorias = new List<CategoriaAlimento>
        {
            new()
            {
                Nombre    = "Cereales y tubérculos",
                Activo    = true,
                Alimentos = new List<Alimento>
                {
                    new() { Nombre = "Arroz",  EdadMinimaIntro = 6,  Recomendacion = "Cocido y bien blando",       Activo = true },
                    new() { Nombre = "Avena",  EdadMinimaIntro = 6,  Recomendacion = "En papilla o colada",        Activo = true },
                    new() { Nombre = "Papa",   EdadMinimaIntro = 6,  Recomendacion = "Cocida y aplastada en puré", Activo = true },
                    new() { Nombre = "Yuca",   EdadMinimaIntro = 8,  Recomendacion = "Cocida y bien triturada",    Activo = true },
                    new() { Nombre = "Pan",    EdadMinimaIntro = 12, Recomendacion = "Sin sal, en trozos pequeños",Activo = true },
                }
            },
            new()
            {
                Nombre    = "Frutas",
                Activo    = true,
                Alimentos = new List<Alimento>
                {
                    new() { Nombre = "Banano",  EdadMinimaIntro = 6,  Recomendacion = "Aplastado o en trocitos", Activo = true },
                    new() { Nombre = "Manzana", EdadMinimaIntro = 6,  Recomendacion = "Rallada o cocida",        Activo = true },
                    new() { Nombre = "Pera",    EdadMinimaIntro = 6,  Recomendacion = "Sin cáscara, en puré",    Activo = true },
                    new() { Nombre = "Papaya",  EdadMinimaIntro = 7,  Recomendacion = "En trozos suaves",        Activo = true },
                    new() { Nombre = "Naranja", EdadMinimaIntro = 10, Recomendacion = "Jugo diluido al inicio",  Activo = true },
                }
            },
            new()
            {
                Nombre    = "Verduras",
                Activo    = true,
                Alimentos = new List<Alimento>
                {
                    new() { Nombre = "Zanahoria", EdadMinimaIntro = 6, Recomendacion = "Cocida y en puré",   Activo = true },
                    new() { Nombre = "Zapallo",   EdadMinimaIntro = 6, Recomendacion = "Cocido y aplastado", Activo = true },
                    new() { Nombre = "Espinaca",  EdadMinimaIntro = 7, Recomendacion = "Cocida, triturada",  Activo = true },
                    new() { Nombre = "Brócoli",   EdadMinimaIntro = 8, Recomendacion = "Al vapor, en trozos",Activo = true },
                }
            },
            new()
            {
                Nombre    = "Proteínas",
                Activo    = true,
                Alimentos = new List<Alimento>
                {
                    new() { Nombre = "Pollo",   EdadMinimaIntro = 7, Recomendacion = "Cocido, desmenuzado",     Activo = true },
                    new() { Nombre = "Res",     EdadMinimaIntro = 7, Recomendacion = "Bien cocida, molida",     Activo = true },
                    new() { Nombre = "Huevo",   EdadMinimaIntro = 8, Recomendacion = "Yema al inicio",          Activo = true },
                    new() { Nombre = "Pescado", EdadMinimaIntro = 9, Recomendacion = "Sin espinas, cocido",     Activo = true },
                    new() { Nombre = "Lenteja", EdadMinimaIntro = 8, Recomendacion = "Bien cocida y aplastada", Activo = true },
                }
            },
            new()
            {
                Nombre    = "Lácteos",
                Activo    = true,
                Alimentos = new List<Alimento>
                {
                    new() { Nombre = "Yogur natural", EdadMinimaIntro = 8,  Recomendacion = "Sin azúcar, entero",     Activo = true },
                    new() { Nombre = "Queso fresco",  EdadMinimaIntro = 10, Recomendacion = "En trocitos pequeños",   Activo = true },
                    new() { Nombre = "Leche de vaca", EdadMinimaIntro = 12, Recomendacion = "Entera, después del año",Activo = true },
                }
            }
        };

            context.CategoriasAlimentos.AddRange(categorias);
            await context.SaveChangesAsync();
        }
    }
}

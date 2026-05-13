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
                    Nombre = "Cereales y tubérculos", Activo = true,
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Arroz",  EdadMinimaMeses = 6,  Recomendacion = "Cocido y bien blando", Descripcion = "Cereal base, sin sal ni condimentos.", Activo = true },
                        new() { Nombre = "Avena",  EdadMinimaMeses = 6,  Recomendacion = "En papilla o colada espesa", Descripcion = "Aporte de fibra y energía duradera.", Activo = true },
                        new() { Nombre = "Papa",   EdadMinimaMeses = 6,  Recomendacion = "Cocida y aplastada en puré", Descripcion = "Tubérculo de fácil digestión.", Activo = true },
                        new() { Nombre = "Camote", EdadMinimaMeses = 6,  Recomendacion = "Asado o cocido en puré", Descripcion = "Rico en vitamina A y sabor dulce natural.", Activo = true },
                        new() { Nombre = "Yuca",   EdadMinimaMeses = 8,  Recomendacion = "Cocida y bien triturada (sin hilos)", Descripcion = "Carbohidrato complejo de gran saciedad.", Activo = true },
                        new() { Nombre = "Plátano Verde", EdadMinimaMeses = 7, Recomendacion = "Cocido y bien aplastado", Descripcion = "Energía pura, ideal para papillas saladas.", Activo = true },
                        new() { Nombre = "Quinua", EdadMinimaMeses = 8,  Recomendacion = "Muy bien cocida (grano abierto)", Descripcion = "Pseudocereal con todos los aminoácidos esenciales.", Activo = true },
                        new() { Nombre = "Maíz (Tierno)", EdadMinimaMeses = 9, Recomendacion = "Triturado o en crema", Descripcion = "Aporte de fibra, cuidar la textura de la cáscara.", Activo = true },
                        new() { Nombre = "Fideo de Trigo", EdadMinimaMeses = 10, Recomendacion = "Pasta pequeña muy suave", Descripcion = "Introducción controlada al trigo.", Activo = true },
                        new() { Nombre = "Pan Integral", EdadMinimaMeses = 12, Recomendacion = "En trozos pequeños para masticar", Descripcion = "Fomenta la motricidad bucal.", Activo = true }
                    }
                },
                new()
                {
                    Nombre = "Frutas", Activo = true,
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Banano",  EdadMinimaMeses = 6,  Recomendacion = "Aplastado o en trocitos", Descripcion = "Potasio y energía rápida.", Activo = true },
                        new() { Nombre = "Manzana", EdadMinimaMeses = 6,  Recomendacion = "Rallada o cocida al vapor", Descripcion = "Fruta clásica de inicio, muy segura.", Activo = true },
                        new() { Nombre = "Pera",    EdadMinimaMeses = 6,  Recomendacion = "Sin cáscara, en puré suave", Descripcion = "Alto contenido de agua y fibra.", Activo = true },
                        new() { Nombre = "Aguacate (Palta)", EdadMinimaMeses = 6, Recomendacion = "Aplastado (textura cremosa)", Descripcion = "Grasas saludables esenciales para el cerebro.", Activo = true },
                        new() { Nombre = "Papaya",  EdadMinimaMeses = 7,  Recomendacion = "En trozos suaves o puré", Descripcion = "Excelente para el tránsito intestinal.", Activo = true },
                        new() { Nombre = "Melón",   EdadMinimaMeses = 8,  Recomendacion = "Trozos muy suaves y jugosos", Descripcion = "Hidratante y refrescante.", Activo = true },
                        new() { Nombre = "Mango",   EdadMinimaMeses = 8,  Recomendacion = "Trozos grandes (estilo BLW) o puré", Descripcion = "Rico en vitaminas A y C.", Activo = true },
                        new() { Nombre = "Sandía",  EdadMinimaMeses = 9,  Recomendacion = "Sin semillas, trozos pequeños", Descripcion = "Ideal para el verano, bajo aporte calórico.", Activo = true },
                        new() { Nombre = "Naranja", EdadMinimaMeses = 10, Recomendacion = "Gajos sin piel ni semillas", Descripcion = "Vitamina C, precaución con la acidez.", Activo = true },
                        new() { Nombre = "Ciruela", EdadMinimaMeses = 10, Recomendacion = "Cocida o puré sin cáscara", Descripcion = "Ayuda en casos de estreñimiento.", Activo = true }
                    }
                },
                new()
                {
                    Nombre = "Verduras", Activo = true,
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Zanahoria", EdadMinimaMeses = 6, Recomendacion = "Cocida y en puré fino", Descripcion = "Aporte de betacarotenos.", Activo = true },
                        new() { Nombre = "Zapallo / Calabaza", EdadMinimaMeses = 6, Recomendacion = "Cocido y aplastado", Descripcion = "Sabor dulce que agrada a los bebés.", Activo = true },
                        new() { Nombre = "Zucchini (Calabacín)", EdadMinimaMeses = 7, Recomendacion = "Cocido sin semillas", Descripcion = "Textura muy suave y ligera.", Activo = true },
                        new() { Nombre = "Espinaca",  EdadMinimaMeses = 8, Recomendacion = "Cocida y bien picada", Descripcion = "Hierro, ofrecer en cantidades moderadas.", Activo = true },
                        new() { Nombre = "Acelga",    EdadMinimaMeses = 8, Recomendacion = "Solo hojas, bien cocidas", Descripcion = "Rica en minerales.", Activo = true },
                        new() { Nombre = "Brócoli",   EdadMinimaMeses = 8, Recomendacion = "Al vapor, solo arbolitos suaves", Descripcion = "Fomenta el agarre y la masticación.", Activo = true },
                        new() { Nombre = "Remolacha", EdadMinimaMeses = 9, Recomendacion = "Cocida y rallada", Descripcion = "Aporte de folatos y color vibrante.", Activo = true },
                        new() { Nombre = "Coliflor",  EdadMinimaMeses = 9, Recomendacion = "Cocida muy suave", Descripcion = "Variedad de sabor y textura.", Activo = true },
                        new() { Nombre = "Arvejas",   EdadMinimaMeses = 10, Recomendacion = "Cocinadas y aplastadas", Descripcion = "Proteína vegetal y fibra.", Activo = true },
                        new() { Nombre = "Vainitas (Judías)", EdadMinimaMeses = 10, Recomendacion = "Cocidas y picadas pequeñas", Descripcion = "Aporte de fibra y vitaminas.", Activo = true }
                    }
                },
                new()
                {
                    Nombre = "Proteínas", Activo = true,
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Pollo (Pechuga)", EdadMinimaMeses = 7, Recomendacion = "Cocido y desmenuzado fino", Descripcion = "Proteína magra de inicio.", Activo = true },
                        new() { Nombre = "Res (Magra)", EdadMinimaMeses = 7, Recomendacion = "Bien cocida y molida", Descripcion = "Fuente principal de hierro hemínico.", Activo = true },
                        new() { Nombre = "Hígado de Pollo", EdadMinimaMeses = 7, Recomendacion = "Cocido y rallado en la sopa", Descripcion = "Bomba de hierro y vitamina A.", Activo = true },
                        new() { Nombre = "Huevo (Yema)", EdadMinimaMeses = 6, Recomendacion = "Bien cocida y desmenuzada", Descripcion = "Grasas y proteínas de alta calidad.", Activo = true },
                        new() { Nombre = "Huevo (Clara)", EdadMinimaMeses = 9, Recomendacion = "Picada muy pequeña", Descripcion = "Introducción gradual para evitar alergias.", Activo = true },
                        new() { Nombre = "Lenteja", EdadMinimaMeses = 8, Recomendacion = "Bien cocida y sin piel", Descripcion = "Leguminosa estrella para el hierro.", Activo = true },
                        new() { Nombre = "Garbanzo", EdadMinimaMeses = 9, Recomendacion = "Cocinados y triturados (hummus)", Descripcion = "Excelente aporte proteico vegetal.", Activo = true },
                        new() { Nombre = "Pescado Blanco", EdadMinimaMeses = 10, Recomendacion = "Al vapor, revisar espinas x3", Descripcion = "Fácil de digerir y rico en fósforo.", Activo = true },
                        new() { Nombre = "Pavo", EdadMinimaMeses = 8, Recomendacion = "Cocido y picado fino", Descripcion = "Alternativa magra al pollo.", Activo = true },
                        new() { Nombre = "Fréjol", EdadMinimaMeses = 10, Recomendacion = "Solo el grano, bien cocido y pelado", Descripcion = "Fibra y proteína vegetal.", Activo = true }
                    }
                },
                new()
                {
                    Nombre = "Lácteos y Otros", Activo = true,
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Yogur Natural", EdadMinimaMeses = 10, Recomendacion = "Sin azúcar, entero (griego)", Descripcion = "Probióticos para la flora intestinal.", Activo = true },
                        new() { Nombre = "Queso Fresco", EdadMinimaMeses = 10, Recomendacion = "Bajo en sal, en cubitos", Descripcion = "Calcio y proteína.", Activo = true },
                        new() { Nombre = "Leche de Vaca", EdadMinimaMeses = 12, Recomendacion = "Entera, no como sustituto", Descripcion = "Solo después del primer año de vida.", Activo = true },
                        new() { Nombre = "Aceite de Oliva", EdadMinimaMeses = 6, Recomendacion = "Una cucharadita cruda sobre puré", Descripcion = "Grasas monoinsaturadas necesarias.", Activo = true },
                        new() { Nombre = "Mantequilla de Maní", EdadMinimaMeses = 12, Recomendacion = "Untada muy fina o en mezclas", Descripcion = "Alérgeno común, introducir con cuidado.", Activo = true }
                    }
                }
            };

            await context.CategoriasAlimentos.AddRangeAsync(categorias);
            await context.SaveChangesAsync();
        }
    }
}
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
                // ── 0-5 meses: Alimentación temprana ─────────────────────────────
                new()
                {
                    Nombre = "Alimentación Temprana", Activo = true,
                    Descripcion = "Nutrición exclusiva del recién nacido hasta los primeros meses de vida.",
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Leche materna",
                            EdadMinimaMeses = 0,
                            Recomendacion = "A libre demanda, sin restricciones de horario",
                            Descripcion = "Alimento ideal y exclusivo del recién nacido. Aporta anticuerpos, nutrientes y vínculo afectivo.", Activo = true },

                        new() { Nombre = "Fórmula de inicio (0-6m)",
                            EdadMinimaMeses = 0,
                            Recomendacion = "Preparar según indicaciones del fabricante; nunca diluir de más",
                            Descripcion = "Sustituto de leche materna para cuando no es posible amamantar. Etapa 1.", Activo = true },

                        new() { Nombre = "Agua hervida tibia",
                            EdadMinimaMeses = 1,
                            Recomendacion = "Solo si el médico lo indica o hace mucho calor",
                            Descripcion = "Aporte mínimo de hidratación complementaria en recién nacidos en ambientes muy cálidos.", Activo = true },

                        new() { Nombre = "Colada de arroz (papilla muy líquida)",
                            EdadMinimaMeses = 3,
                            Recomendacion = "Muy líquida (1 cucharada de arroz por 200ml de agua), sin sal",
                            Descripcion = "Primera papilla de cereal. Textura casi líquida para facilitar la deglución antes de los 4 meses.", Activo = true },

                        new() { Nombre = "Papilla de avena (inicio)",
                            EdadMinimaMeses = 4,
                            Recomendacion = "Hojuelas de avena finas disueltas en agua o leche materna, sin azúcar",
                            Descripcion = "Cereal con fibra soluble de fácil digestión para bebés de 4 meses. Aporte energético sostenido.", Activo = true },

                        new() { Nombre = "Papilla de maicena",
                            EdadMinimaMeses = 4,
                            Recomendacion = "Muy suave, sin grumos, sin sal ni azúcar",
                            Descripcion = "Maicena cocida en agua o leche materna. Textura cremosa ideal para primeras cucharadas.", Activo = true },

                        new() { Nombre = "Caldo de verduras colado",
                            EdadMinimaMeses = 4,
                            Recomendacion = "Solo el líquido, sin sal ni condimentos; servir tibio",
                            Descripcion = "Caldo suave de zanahoria, papa y calabaza. Introduce sabores nuevos y aporta minerales.", Activo = true },

                        new() { Nombre = "Fórmula de seguimiento (6-12m)",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Complementa la alimentación sólida, no la reemplaza",
                            Descripcion = "Fórmula Etapa 2 diseñada para acompañar la alimentación complementaria.", Activo = true },
                    }
                },

                // ── Cereales y tubérculos ────────────────────────────────────────
                new()
                {
                    Nombre = "Cereales y tubérculos", Activo = true,
                    Descripcion = "Base energética de la dieta infantil. Introducir en texturas progresivas.",
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Arroz blanco",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Muy bien cocido, casi en papilla; sin sal",
                            Descripcion = "Cereal base de fácil digestión. Ideal para primeras papillas espesas.", Activo = true },

                        new() { Nombre = "Avena en hojuelas",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Papilla espesa o colada con leche materna",
                            Descripcion = "Fibra soluble y energía de larga duración. Una de las mejores opciones para el desayuno.", Activo = true },

                        new() { Nombre = "Papa",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Cocida y aplastada en puré fino sin grumos",
                            Descripcion = "Tubérculo suave y de sabor neutro. Excelente para mezclar con verduras.", Activo = true },

                        new() { Nombre = "Camote (batata)",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Asado o cocido al vapor; aplastado en puré",
                            Descripcion = "Dulzor natural que agrada a los bebés. Rico en vitamina A y betacarotenos.", Activo = true },

                        new() { Nombre = "Plátano verde",
                            EdadMinimaMeses = 7,
                            Recomendacion = "Bien cocido y aplastado; agregar aceite de oliva",
                            Descripcion = "Almidón resistente que nutre la flora intestinal. Base de sopas y purés salados.", Activo = true },

                        new() { Nombre = "Yuca",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Cocida, sin fibras duras, bien triturada",
                            Descripcion = "Carbohidrato de alta energía. Retirar los hilos antes de servir.", Activo = true },

                        new() { Nombre = "Quinua",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Grano muy cocido (abierto) en sopas o papillas",
                            Descripcion = "Pseudocereal con los nueve aminoácidos esenciales. Excelente para bebés.", Activo = true },

                        new() { Nombre = "Maíz tierno (choclo)",
                            EdadMinimaMeses = 9,
                            Recomendacion = "Cocido y triturado en crema; retirar la cáscara",
                            Descripcion = "Aporte de fibra y sabor dulce. Evitar el grano entero hasta los 2 años.", Activo = true },

                        new() { Nombre = "Fideo de trigo (pasta pequeña)",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Estrellitas o fideos rotos muy bien cocidos",
                            Descripcion = "Introducción al trigo con formatos pequeños. Revisar tolerancia individual.", Activo = true },

                        new() { Nombre = "Pan blanco suave",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Miga blanda, sin corteza; supervisar siempre al comer",
                            Descripcion = "Ideal para mordisquear y estimular la masticación. Introducir en trozos manejables.", Activo = true },

                        new() { Nombre = "Pan integral",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Trozos pequeños; fomenta la masticación autónoma",
                            Descripcion = "Mayor fibra que el pan blanco. Parte del desayuno familiar.", Activo = true },

                        new() { Nombre = "Arroz con leche (sin azúcar)",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Sin azúcar añadida; con leche entera de vaca o fórmula",
                            Descripcion = "Preparación tradicional adaptada. Fuente de carbohidratos y calcio.", Activo = true },

                        new() { Nombre = "Galleta de arroz sin sal",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Una por vez, supervisado; disuelve fácilmente",
                            Descripcion = "Snack seguro para bebés de 12 meses. Estimula masticación.", Activo = true },

                        new() { Nombre = "Tapioca (perlas de yuca)",
                            EdadMinimaMeses = 14,
                            Recomendacion = "Muy cocida, en bebidas o postres sin azúcar",
                            Descripcion = "Digestión fácil. Textura distinta que introduce variedad.", Activo = true },

                        new() { Nombre = "Maíz (palomitas blandas sin sal)",
                            EdadMinimaMeses = 18,
                            Recomendacion = "Solo las palomitas que se disuelven, sin condimentos",
                            Descripcion = "Para niños que ya mastica bien. Nunca dejar sin supervisión.", Activo = true },

                        new() { Nombre = "Granola sin miel (2 años)",
                            EdadMinimaMeses = 24,
                            Recomendacion = "Mezclada con yogur; sin miel hasta los 2 años",
                            Descripcion = "Cereal completo con avena, frutas y semillas. Introducir al completar 2 años.", Activo = true },
                    }
                },

                // ── Frutas ───────────────────────────────────────────────────────
                new()
                {
                    Nombre = "Frutas", Activo = true,
                    Descripcion = "Vitaminas, fibra y azúcares naturales. Ofrecer sin azúcar añadida.",
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Puré de manzana",
                            EdadMinimaMeses = 4,
                            Recomendacion = "Cocida al vapor y aplastada; sin cáscara ni semillas",
                            Descripcion = "Primera fruta de inicio. Sabor suave, fibra soluble y muy bien tolerada.", Activo = true },

                        new() { Nombre = "Puré de pera",
                            EdadMinimaMeses = 4,
                            Recomendacion = "Cocida o madura y aplastada; sin cáscara",
                            Descripcion = "Alto contenido de agua y fibra. Ayuda a regular el tránsito intestinal.", Activo = true },

                        new() { Nombre = "Banano / Plátano maduro",
                            EdadMinimaMeses = 5,
                            Recomendacion = "Muy maduro, aplastado con tenedor o en rodajas suaves",
                            Descripcion = "Potasio y energía rápida. Textura cremosa ideal para primeras papillas de frutas.", Activo = true },

                        new() { Nombre = "Aguacate (palta)",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Aplastado; textura cremosa natural, sin condimentos",
                            Descripcion = "Grasas monoinsaturadas esenciales para el desarrollo cerebral. Muy nutritivo.", Activo = true },

                        new() { Nombre = "Manzana en trocitos",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Pelada y en bastones o trozos suaves para agarrar",
                            Descripcion = "Versión sólida para bebés que ya mastican. Progresión desde el puré.", Activo = true },

                        new() { Nombre = "Papaya",
                            EdadMinimaMeses = 7,
                            Recomendacion = "En trozos suaves o puré; sin semillas",
                            Descripcion = "Excelente para la digestión. Contiene papaína que facilita la absorción de proteínas.", Activo = true },

                        new() { Nombre = "Mango",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Maduro; en puré o bastones grandes estilo BLW",
                            Descripcion = "Rico en vitaminas A y C. Introducir bien maduro para menor acidez.", Activo = true },

                        new() { Nombre = "Melón",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Trozos suaves y jugosos sin cáscara ni semillas",
                            Descripcion = "Altamente hidratante. Bajo aporte calórico, bueno para el verano.", Activo = true },

                        new() { Nombre = "Sandía",
                            EdadMinimaMeses = 9,
                            Recomendacion = "Sin semillas; en bastones para agarrar o en cubos pequeños",
                            Descripcion = "Agua natural y licopeno. Ideal para hidratarse en días calurosos.", Activo = true },

                        new() { Nombre = "Naranja (gajos)",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Gajos sin piel, sin semillas; vigilar acidez",
                            Descripcion = "Vitamina C que potencia la absorción de hierro. Puede irritar si hay sensibilidad.", Activo = true },

                        new() { Nombre = "Ciruela cocida",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Cocida y en puré sin cáscara ni hueso",
                            Descripcion = "Natural regulador del tránsito intestinal. Muy útil en estreñimiento.", Activo = true },

                        new() { Nombre = "Durazno / Melocotón",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Sin cáscara ni hueso; en trozos blandos o puré",
                            Descripcion = "Fruta de temporada con vitaminas A y C. Textura suave y sabor agradable.", Activo = true },

                        new() { Nombre = "Uvas sin cáscara",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Cortadas en cuartos a lo largo; NUNCA enteras (riesgo de asfixia)",
                            Descripcion = "Antioxidantes y azúcar natural. El corte en cuartos es obligatorio.", Activo = true },

                        new() { Nombre = "Fresa",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Bien madura, en trozos; introducir de a poco (alérgeno potencial)",
                            Descripcion = "Vitamina C y antioxidantes. Algunos bebés pueden mostrar urticaria leve.", Activo = true },

                        new() { Nombre = "Kiwi",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Sin cáscara, en trozos pequeños; observar tolerancia",
                            Descripcion = "Alta concentración de vitamina C. Puede ser ácido; introducir gradualmente.", Activo = true },

                        new() { Nombre = "Piña",
                            EdadMinimaMeses = 14,
                            Recomendacion = "Trozos muy maduros y suaves; en pequeñas cantidades",
                            Descripcion = "Bromelina que facilita digestión. Puede irritar la mucosa en exceso.", Activo = true },

                        new() { Nombre = "Maracuyá (zumo diluido)",
                            EdadMinimaMeses = 18,
                            Recomendacion = "Muy diluido en agua (1:4); sin azúcar añadida",
                            Descripcion = "Vitaminas y antioxidantes. Alta acidez: siempre diluir.", Activo = true },
                    }
                },

                // ── Verduras ─────────────────────────────────────────────────────
                new()
                {
                    Nombre = "Verduras", Activo = true,
                    Descripcion = "Vitaminas, minerales y fibra. Ofrecer cocidas y sin condimentos al inicio.",
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Zanahoria cocida",
                            EdadMinimaMeses = 5,
                            Recomendacion = "Al vapor y aplastada; en puré fino al inicio",
                            Descripcion = "Betacarotenos (vitamina A). Una de las primeras verduras recomendadas.", Activo = true },

                        new() { Nombre = "Zapallo / Calabaza",
                            EdadMinimaMeses = 5,
                            Recomendacion = "Cocido al vapor o en horno; aplastado",
                            Descripcion = "Sabor dulce muy aceptado por los bebés. Vitamina A y textura sedosa.", Activo = true },

                        new() { Nombre = "Zucchini (calabacín)",
                            EdadMinimaMeses = 7,
                            Recomendacion = "Al vapor sin semillas; en bastones para BLW o en puré",
                            Descripcion = "Textura muy suave tras cocción. Bajo en alérgenos, alta tolerabilidad.", Activo = true },

                        new() { Nombre = "Espinaca",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Cocida y bien picada; mezclar con papillas",
                            Descripcion = "Hierro y ácido fólico. Ofrecer en cantidades moderadas y no cada día.", Activo = true },

                        new() { Nombre = "Acelga",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Solo las hojas, bien cocidas y picadas finas",
                            Descripcion = "Rica en magnesio y calcio. Evitar el tallo grueso al inicio.", Activo = true },

                        new() { Nombre = "Brócoli",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Al vapor; arbolitos suaves para agarrar (BLW) o en crema",
                            Descripcion = "Hierro, vitamina C y antioxidantes. Puede causar gases; introducir poco a poco.", Activo = true },

                        new() { Nombre = "Remolacha",
                            EdadMinimaMeses = 9,
                            Recomendacion = "Bien cocida y rallada o en puré; puede teñir la orina (normal)",
                            Descripcion = "Folatos y hierro. El color rosado en pañal no es señal de alarma.", Activo = true },

                        new() { Nombre = "Coliflor",
                            EdadMinimaMeses = 9,
                            Recomendacion = "Al vapor muy suave; en crema o purés mezclados",
                            Descripcion = "Vitamina C y fibra. Puede generar gases; combinar con zanahoria.", Activo = true },

                        new() { Nombre = "Arvejas (chícharos)",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Cocidas y aplastadas o pasadas por colador",
                            Descripcion = "Proteína vegetal y fibra. La textura de la cáscara se elimina al pasar por colador.", Activo = true },

                        new() { Nombre = "Vainitas (judías verdes)",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Cocidas y picadas en trozos pequeños",
                            Descripcion = "Fibra, vitaminas K y C. Fáciles de agarrar una vez cocidas.", Activo = true },

                        new() { Nombre = "Tomate (sin piel ni semillas)",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Escaldado, sin piel ni semillas; en salsa natural o en trozos",
                            Descripcion = "Licopeno y vitamina C. Introducir cuando la mucosa digestiva está más madura.", Activo = true },

                        new() { Nombre = "Pepino",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Sin cáscara, sin semillas; en bastones para masticar",
                            Descripcion = "Muy hidratante. Textura crujiente que estimula la masticación.", Activo = true },

                        new() { Nombre = "Cebolla cocida",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Muy bien cocida (pochada); evitar cruda",
                            Descripcion = "Aporta sabor a los guisos. La cocción elimina la acidez y facilita la digestión.", Activo = true },

                        new() { Nombre = "Ajo (en pequeñas cantidades)",
                            EdadMinimaMeses = 14,
                            Recomendacion = "Solo como condimento en guisos; muy poca cantidad",
                            Descripcion = "Propiedades antibacterianas. Jamás crudo; en cocción suaviza su fuerza.", Activo = true },

                        new() { Nombre = "Pimiento rojo cocido",
                            EdadMinimaMeses = 15,
                            Recomendacion = "Asado o cocido, sin piel ni semillas; en tiras",
                            Descripcion = "Altísimo contenido de vitamina C. El rojo es más dulce y menos irritante.", Activo = true },

                        new() { Nombre = "Maíz (granos enteros)",
                            EdadMinimaMeses = 18,
                            Recomendacion = "Tierno y bien cocido; masticación supervisada",
                            Descripcion = "A partir de los 18 meses con buena masticación. Antes, solo en crema.", Activo = true },
                    }
                },

                // ── Proteínas ────────────────────────────────────────────────────
                new()
                {
                    Nombre = "Proteínas", Activo = true,
                    Descripcion = "Aminoácidos esenciales para el crecimiento muscular y cerebral.",
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Huevo (yema cocida)",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Yema dura desmenuzada; mezclar con puré de verduras",
                            Descripcion = "Colina y grasas esenciales para el desarrollo cerebral. Una de las primeras proteínas.", Activo = true },

                        new() { Nombre = "Pollo (pechuga desmenuzada)",
                            EdadMinimaMeses = 7,
                            Recomendacion = "Cocida al vapor y desmenuzada muy fina; sin piel ni huesos",
                            Descripcion = "Proteína magra y digestión fácil. Primera carne recomendada por la mayoría de pediatras.", Activo = true },

                        new() { Nombre = "Res magra (molida)",
                            EdadMinimaMeses = 7,
                            Recomendacion = "Bien cocida y molida; mezclar con purés",
                            Descripcion = "Fuente principal de hierro hemínico. Clave para prevenir anemia.", Activo = true },

                        new() { Nombre = "Hígado de pollo",
                            EdadMinimaMeses = 7,
                            Recomendacion = "Cocido y rallado fino en sopa; máx 1 vez por semana",
                            Descripcion = "Concentración altísima de hierro y vitamina A. No exceder la frecuencia semanal.", Activo = true },

                        new() { Nombre = "Pavo (pechuga)",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Cocido y desmenuzado; alternativa al pollo",
                            Descripcion = "Carne blanca baja en grasa. Variedad en texturas y sabores proteicos.", Activo = true },

                        new() { Nombre = "Lenteja",
                            EdadMinimaMeses = 8,
                            Recomendacion = "Bien cocida, sin cáscara exterior (colada); en sopa o crema",
                            Descripcion = "Leguminosa estrella en hierro y proteína vegetal. Combinar con vitamina C.", Activo = true },

                        new() { Nombre = "Huevo (clara cocida)",
                            EdadMinimaMeses = 9,
                            Recomendacion = "Introducir gradualmente; vigilar signos de alergia",
                            Descripcion = "Proteína de alta calidad. La clara es alérgeno potencial; introducir después de la yema.", Activo = true },

                        new() { Nombre = "Garbanzo (hummus o aplastado)",
                            EdadMinimaMeses = 9,
                            Recomendacion = "Cocido y triturado con aceite de oliva; sin tahini al inicio",
                            Descripcion = "Proteína vegetal, hierro y zinc. El hummus casero es la presentación ideal.", Activo = true },

                        new() { Nombre = "Pescado blanco (corvina, tilapia)",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Al vapor; verificar ausencia de espinas tres veces",
                            Descripcion = "Fósforo, yodo y ácidos grasos omega-3. Digestión más fácil que pescados azules.", Activo = true },

                        new() { Nombre = "Fréjol / Frijol",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Solo el grano, muy bien cocido y pelado; en puré",
                            Descripcion = "Fibra y proteína vegetal. Cocción larga reduce antinutrientes.", Activo = true },

                        new() { Nombre = "Huevo entero (revuelto blando)",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Revuelto blando sin aceite en exceso; bien cocido",
                            Descripcion = "Una vez superado el año, el huevo completo es un alimento muy completo.", Activo = true },

                        new() { Nombre = "Atún en agua (escurrido)",
                            EdadMinimaMeses = 12,
                            Recomendacion = "En agua, escurrido; máx 2 veces por semana por el mercurio",
                            Descripcion = "Omega-3 y proteína. Limitar frecuencia por contenido moderado de mercurio.", Activo = true },

                        new() { Nombre = "Sardina (en agua, sin sal)",
                            EdadMinimaMeses = 14,
                            Recomendacion = "Desmigada y sin espinas; ideal mezclada con pasta o puré",
                            Descripcion = "Alto en omega-3 y calcio (espinas blandas). Una opción muy nutritiva y económica.", Activo = true },

                        new() { Nombre = "Cerdo magro (lomo)",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Bien cocido (temperatura interna segura); en tiras suaves",
                            Descripcion = "Vitaminas del grupo B y zinc. Lomo o solomillo son los cortes más magros.", Activo = true },

                        new() { Nombre = "Tofu (cuajada de soya)",
                            EdadMinimaMeses = 14,
                            Recomendacion = "Suave (silken tofu), cocido; observar tolerancia a la soya",
                            Descripcion = "Proteína vegetal completa. Excelente alternativa para familias vegetarianas.", Activo = true },

                        new() { Nombre = "Salmón (al vapor)",
                            EdadMinimaMeses = 18,
                            Recomendacion = "Al vapor sin piel; sin sal; revisar espinas",
                            Descripcion = "Omega-3 DHA esencial para el cerebro. Máx 2 veces por semana.", Activo = true },
                    }
                },

                // ── Lácteos y otros ──────────────────────────────────────────────
                new()
                {
                    Nombre = "Lácteos y otros", Activo = true,
                    Descripcion = "Calcio, vitaminas y grasas necesarias. Introducir según la edad indicada.",
                    Alimentos = new List<Alimento>
                    {
                        new() { Nombre = "Aceite de oliva virgen extra",
                            EdadMinimaMeses = 6,
                            Recomendacion = "Una cucharadita cruda sobre purés o papillas; no calentar",
                            Descripcion = "Grasas monoinsaturadas imprescindibles para el desarrollo neurológico.", Activo = true },

                        new() { Nombre = "Yogur natural entero",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Sin azúcar, sin saborizantes; tipo griego o entero",
                            Descripcion = "Probióticos para la flora intestinal y calcio. Más tolerado que la leche de vaca.", Activo = true },

                        new() { Nombre = "Queso fresco bajo en sal",
                            EdadMinimaMeses = 10,
                            Recomendacion = "En cubitos pequeños; verificar contenido de sal (< 1g/100g)",
                            Descripcion = "Calcio y proteína. El queso fresco tiene menos sal que los curados.", Activo = true },

                        new() { Nombre = "Leche de vaca entera",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Solo entera (3.5% grasa); nunca desnatada antes de los 2 años",
                            Descripcion = "Calcio y vitamina D. No reemplazar la leche materna o fórmula antes del año.", Activo = true },

                        new() { Nombre = "Mantequilla sin sal",
                            EdadMinimaMeses = 10,
                            Recomendacion = "Untada muy finamente en pan o mezclada en purés",
                            Descripcion = "Grasas saturadas y vitaminas liposolubles. En pequeñas cantidades.", Activo = true },

                        new() { Nombre = "Queso maduro rallado",
                            EdadMinimaMeses = 18,
                            Recomendacion = "Rallado sobre pastas o sopas; muy poca cantidad por el sodio",
                            Descripcion = "Mayor concentración de calcio que el queso fresco. Alto en sodio; usar con moderación.", Activo = true },

                        new() { Nombre = "Crema de cacahuete / maní (sin sal)",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Untada muy fina en pan o diluida; NUNCA en cuchara sola (riesgo de asfixia)",
                            Descripcion = "Proteína, grasas saludables y vitamina E. Alérgeno potencial: introducir gradualmente.", Activo = true },

                        new() { Nombre = "Aceite de coco virgen",
                            EdadMinimaMeses = 12,
                            Recomendacion = "Cucharadita para cocinar; no abusar por grasas saturadas",
                            Descripcion = "Ácido láurico con propiedades antimicrobianas. Alternativa al aceite de oliva.", Activo = true },

                        new() { Nombre = "Leche de almendras (sin azúcar)",
                            EdadMinimaMeses = 18,
                            Recomendacion = "Solo como complemento, nunca como sustituto de leche materna o vaca",
                            Descripcion = "Opción vegetal para familias que evitan lácteos. Verificar que esté enriquecida en calcio.", Activo = true },

                        new() { Nombre = "Miel (a partir de 2 años)",
                            EdadMinimaMeses = 24,
                            Recomendacion = "NUNCA antes de los 2 años (riesgo de botulismo infantil)",
                            Descripcion = "Endulzante natural permitido solo después de los 24 meses. Antes: prohibida.", Activo = true },
                    }
                }
            };

            await context.CategoriasAlimentos.AddRangeAsync(categorias);
            await context.SaveChangesAsync();
        }
    }
}

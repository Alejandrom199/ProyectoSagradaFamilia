using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class OmsSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.OmsReferencias.AnyAsync())
                return;

            var datos = new List<OmsReferencia>();

            AgregarPesoM(datos);
            AgregarPesoF(datos);
            AgregarTallaM(datos);
            AgregarTallaF(datos);

            await context.OmsReferencias.AddRangeAsync(datos);
            await context.SaveChangesAsync();
        }

        private static void AgregarPesoM(List<OmsReferencia> datos)
        {
            var tipo = TipoReferencia.Peso;
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 0, Tipo = tipo, Percentil3 = 2.4m, Percentil15 = 2.9m, Percentil50 = 3.3m, Percentil85 = 3.9m, Percentil97 = 4.4m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 2, Tipo = tipo, Percentil3 = 4.3m, Percentil15 = 4.9m, Percentil50 = 5.6m, Percentil85 = 6.3m, Percentil97 = 7.1m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 4, Tipo = tipo, Percentil3 = 5.6m, Percentil15 = 6.2m, Percentil50 = 7.0m, Percentil85 = 7.8m, Percentil97 = 8.7m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 6, Tipo = tipo, Percentil3 = 6.4m, Percentil15 = 7.1m, Percentil50 = 7.9m, Percentil85 = 8.8m, Percentil97 = 9.8m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 8, Tipo = tipo, Percentil3 = 7.0m, Percentil15 = 7.7m, Percentil50 = 8.6m, Percentil85 = 9.6m, Percentil97 = 10.7m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 10, Tipo = tipo, Percentil3 = 7.5m, Percentil15 = 8.2m, Percentil50 = 9.2m, Percentil85 = 10.2m, Percentil97 = 11.4m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 12, Tipo = tipo, Percentil3 = 7.8m, Percentil15 = 8.6m, Percentil50 = 9.6m, Percentil85 = 10.8m, Percentil97 = 12.0m });
        }

        private static void AgregarPesoF(List<OmsReferencia> datos)
        {
            var tipo = TipoReferencia.Peso;
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 0, Tipo = tipo, Percentil3 = 2.3m, Percentil15 = 2.8m, Percentil50 = 3.2m, Percentil85 = 3.7m, Percentil97 = 4.2m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 2, Tipo = tipo, Percentil3 = 3.9m, Percentil15 = 4.5m, Percentil50 = 5.1m, Percentil85 = 5.8m, Percentil97 = 6.6m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 4, Tipo = tipo, Percentil3 = 5.0m, Percentil15 = 5.7m, Percentil50 = 6.4m, Percentil85 = 7.3m, Percentil97 = 8.2m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 6, Tipo = tipo, Percentil3 = 5.8m, Percentil15 = 6.6m, Percentil50 = 7.3m, Percentil85 = 8.2m, Percentil97 = 9.3m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 8, Tipo = tipo, Percentil3 = 6.3m, Percentil15 = 7.0m, Percentil50 = 7.9m, Percentil85 = 8.9m, Percentil97 = 10.2m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 10, Tipo = tipo, Percentil3 = 6.8m, Percentil15 = 7.5m, Percentil50 = 8.5m, Percentil85 = 9.6m, Percentil97 = 10.9m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 12, Tipo = tipo, Percentil3 = 7.1m, Percentil15 = 7.9m, Percentil50 = 8.9m, Percentil85 = 10.1m, Percentil97 = 11.5m });
        }

        private static void AgregarTallaM(List<OmsReferencia> datos)
        {
            var tipo = TipoReferencia.Talla;
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 0, Tipo = tipo, Percentil3 = 46.1m, Percentil15 = 48.0m, Percentil50 = 49.9m, Percentil85 = 51.8m, Percentil97 = 53.7m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 2, Tipo = tipo, Percentil3 = 54.4m, Percentil15 = 56.4m, Percentil50 = 58.4m, Percentil85 = 60.4m, Percentil97 = 62.4m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 4, Tipo = tipo, Percentil3 = 60.0m, Percentil15 = 61.9m, Percentil50 = 63.9m, Percentil85 = 65.9m, Percentil97 = 67.8m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 6, Tipo = tipo, Percentil3 = 63.3m, Percentil15 = 65.5m, Percentil50 = 67.6m, Percentil85 = 69.8m, Percentil97 = 71.9m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 8, Tipo = tipo, Percentil3 = 66.2m, Percentil15 = 68.4m, Percentil50 = 70.6m, Percentil85 = 72.8m, Percentil97 = 75.0m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 10, Tipo = tipo, Percentil3 = 68.7m, Percentil15 = 71.0m, Percentil50 = 73.3m, Percentil85 = 75.6m, Percentil97 = 77.9m });
            datos.Add(new OmsReferencia { Sexo = 'M', EdadMeses = 12, Tipo = tipo, Percentil3 = 71.0m, Percentil15 = 73.4m, Percentil50 = 75.7m, Percentil85 = 78.1m, Percentil97 = 80.5m });
        }

        private static void AgregarTallaF(List<OmsReferencia> datos)
        {
            var tipo = TipoReferencia.Talla;
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 0, Tipo = tipo, Percentil3 = 45.4m, Percentil15 = 47.3m, Percentil50 = 49.1m, Percentil85 = 51.0m, Percentil97 = 52.9m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 2, Tipo = tipo, Percentil3 = 53.0m, Percentil15 = 55.0m, Percentil50 = 57.1m, Percentil85 = 59.1m, Percentil97 = 61.1m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 4, Tipo = tipo, Percentil3 = 58.0m, Percentil15 = 60.0m, Percentil50 = 62.1m, Percentil85 = 64.2m, Percentil97 = 66.2m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 6, Tipo = tipo, Percentil3 = 61.2m, Percentil15 = 63.5m, Percentil50 = 65.7m, Percentil85 = 68.0m, Percentil97 = 70.3m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 8, Tipo = tipo, Percentil3 = 64.0m, Percentil15 = 66.4m, Percentil50 = 68.7m, Percentil85 = 71.1m, Percentil97 = 73.5m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 10, Tipo = tipo, Percentil3 = 66.5m, Percentil15 = 69.0m, Percentil50 = 71.5m, Percentil85 = 73.9m, Percentil97 = 76.4m });
            datos.Add(new OmsReferencia { Sexo = 'F', EdadMeses = 12, Tipo = tipo, Percentil3 = 68.9m, Percentil15 = 71.4m, Percentil50 = 74.0m, Percentil85 = 76.6m, Percentil97 = 79.2m });
        }
    }
}
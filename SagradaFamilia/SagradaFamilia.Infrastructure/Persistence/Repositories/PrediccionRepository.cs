using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class PrediccionRepository : IPrediccionRepository
    {
        private readonly AppDbContext _context;

        public PrediccionRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Prediccion>> ObtenerPorNinoAsync(int ninoId) =>
            await _context.Predicciones
                .Where(p => p.NinoId == ninoId)
                .OrderBy(p => p.FechaObjetivo)
                .ThenBy(p => p.Tipo) 
                .ToListAsync();

        public async Task<Prediccion?> ObtenerPorNinoYFechaAsync(int ninoId, DateOnly fechaObjetivo, TipoReferencia tipo) =>
            await _context.Predicciones
                .FirstOrDefaultAsync(p => p.NinoId == ninoId
                                       && p.FechaObjetivo == fechaObjetivo
                                       && p.Tipo == tipo);

        public async Task GuardarPrediccionesAsync(IEnumerable<Prediccion> predicciones)
        {
            foreach (var prediccion in predicciones)
            {
                var existente = await ObtenerPorNinoYFechaAsync(
                    prediccion.NinoId,
                    prediccion.FechaObjetivo,
                    prediccion.Tipo);

                if (existente is not null)
                {
                    existente.ValorPredicho = prediccion.ValorPredicho;
                    existente.ValorMinimo = prediccion.ValorMinimo;
                    existente.ValorMaximo = prediccion.ValorMaximo;
                    existente.FechaCalculo = DateTime.UtcNow;

                    _context.Predicciones.Update(existente);
                }
                else
                {
                    _context.Predicciones.Add(prediccion);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ActualizarValorRealAsync(int ninoId, DateOnly fechaMedicion, decimal valorReal, TipoReferencia tipo)
        {
            var predicciones = await _context.Predicciones
                .Where(p => p.NinoId == ninoId
                         && p.Tipo == tipo
                         && p.FechaObjetivo.Year == fechaMedicion.Year
                         && p.FechaObjetivo.Month == fechaMedicion.Month)
                .ToListAsync();

            if (!predicciones.Any()) return;

            predicciones.ForEach(p => p.ValorReal = valorReal);
            await _context.SaveChangesAsync();
        }
    }
}
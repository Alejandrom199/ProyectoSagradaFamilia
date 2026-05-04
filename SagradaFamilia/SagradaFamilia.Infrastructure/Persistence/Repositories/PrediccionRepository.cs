using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .ToListAsync();

        public async Task<Prediccion?> ObtenerPorNinoYFechaAsync(int ninoId, DateOnly fechaObjetivo) =>
            await _context.Predicciones
                .FirstOrDefaultAsync(p => p.NinoId == ninoId
                                       && p.FechaObjetivo == fechaObjetivo);

        public async Task GuardarPrediccionesAsync(IEnumerable<Prediccion> predicciones)
        {
            foreach (var prediccion in predicciones)
            {
                var existente = await ObtenerPorNinoYFechaAsync(
                    prediccion.NinoId,
                    prediccion.FechaObjetivo);

                if (existente is not null)
                {
                    existente.PesoPredicho = prediccion.PesoPredicho;
                    existente.PesoMinimo = prediccion.PesoMinimo;
                    existente.PesoMaximo = prediccion.PesoMaximo;
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

        public async Task ActualizarPesoRealAsync(int ninoId, DateOnly fechaMedicion, decimal pesoReal)
        {
            var predicciones = await _context.Predicciones
                .Where(p => p.NinoId == ninoId
                         && p.FechaObjetivo.Year == fechaMedicion.Year
                         && p.FechaObjetivo.Month == fechaMedicion.Month)
                .ToListAsync();

            if (!predicciones.Any()) return;

            predicciones.ForEach(p => p.PesoReal = pesoReal);
            await _context.SaveChangesAsync();
        }
    }
}

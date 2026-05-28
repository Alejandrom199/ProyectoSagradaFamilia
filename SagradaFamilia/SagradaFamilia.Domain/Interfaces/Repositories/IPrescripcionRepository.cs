using SagradaFamilia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IPrescripcionRepository
    {
        Task<Prescripcion?> ObtenerPorIdAsync(int id);

        Task<IEnumerable<Prescripcion>> ObtenerHistorialPorNinoAsync(int ninoId);
        Task<IEnumerable<Prescripcion>> ObtenerPorMedicoAsync(int medicoId);

        Task<(IEnumerable<Prescripcion> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending);
        Task<Prescripcion> CrearAsync(Prescripcion prescripcion);
        Task<Prescripcion> ActualizarAsync(Prescripcion prescripcion);
        Task EliminarAsync(int id);
    }
}

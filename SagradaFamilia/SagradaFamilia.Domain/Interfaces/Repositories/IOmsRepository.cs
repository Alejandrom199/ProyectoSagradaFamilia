using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IOmsRepository
    {
        /// <summary>
        /// Obtiene los percentiles de referencia para un sexo, edad y tipo específico.
        /// </summary>
        Task<OmsReferencia?> ObtenerReferenciaAsync(char sexo, int edadMeses, TipoReferencia tipo);
        
        /// <summary>
        /// Obtiene la lista completa de percentiles (0-60 meses) para graficar 
        /// las curvas de crecimiento en el frontend.
        /// </summary>
        Task<IEnumerable<OmsReferencia>> ObtenerCurvaCompletaAsync(char sexo, TipoReferencia tipo);
    }
}

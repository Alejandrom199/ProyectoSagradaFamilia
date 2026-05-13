using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IOmsService
    {
        Task<OmsDto.Response?> ObtenerReferenciaAsync(char sexo, int edadMeses, TipoReferencia tipo);

        Task<IEnumerable<OmsDto.Response>> ObtenerCurvaCompletaAsync(char sexo, TipoReferencia tipo);
        
        /// <summary>
        /// Compara un valor real (peso o talla) contra los percentiles de la OMS 
        /// y devuelve el diagnóstico (ej. "Desnutrición", "Normal", "Sobrepeso").
        /// Este método será llamado por IMedidaService al crear una nueva medida.
        /// </summary>
        Task<string> CalcularEstadoNutricionalAsync(char sexo, int edadMeses, decimal valorMedido, TipoReferencia tipo);

        /// <summary>
        /// Devuelve en qué percentil exacto (o rango) cae la medida del paciente.
        /// </summary>
        Task<int> DeterminarPercentilAsync(char sexo, int edadMeses, decimal valorMedido, TipoReferencia tipo);
    }
}
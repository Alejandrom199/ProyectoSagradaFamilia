using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IOmsRepository
    {
        Task<OmsPesoPorEdad?> ObtenerPesoPorEdadAsync(char sexo, int edadMeses);
        Task<OmsTallaPorEdad?> ObtenerTallaPorEdadAsync(char sexo, int edadMeses);
    }
}

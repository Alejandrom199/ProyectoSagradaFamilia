using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface ICatalogoValorService
    {
        Task<IEnumerable<CatalogoValorDto.Response>> ObtenerTodosAsync();
        Task<IEnumerable<CatalogoValorDto.Response>> ObtenerPorTipoAsync(string tipo);
        Task<CatalogoValorDto.Response?> ObtenerPorTipoYCodigoAsync(string tipo, string codigo);
        Task<CatalogoValorDto.Response> ObtenerPorIdAsync(int id);
        Task<CatalogoValorDto.Response> CrearAsync(CatalogoValorDto.Create request);
        Task<CatalogoValorDto.Response> ActualizarAsync(int id, CatalogoValorDto.Update request);
        Task EliminarAsync(int id);
    }
}

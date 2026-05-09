namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IReportesService
    {
        Task<byte[]> GenerarAlimentosPdf(string? titulo);
        Task<byte[]> GenerarPadresPdf(string? titulo);
    }
}

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IReportesService
    {
        Task<byte[]> GenerarAlimentosPdf(string? titulo, string logoPath, string marcaAguaPath);
        Task<byte[]> GenerarPadresPdf(string? titulo, string logoPath, string marcaAguaPath);
    }
}

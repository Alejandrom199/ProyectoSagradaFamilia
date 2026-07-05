namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IReportesService
    {
        Task<byte[]> GenerarAlimentosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarPadresPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarNinosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarMedicosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarUsuariosPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarMedidasPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarPrescripcionesPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarCitasNinoPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarCitasMedicoPdf(int usuarioId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarPrescripcionesMedicoPdf(int usuarioId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarHistoriaClinicaPdf(int ninoId, string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarLogsPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
        Task<byte[]> GenerarAuditoriaPdf(string? titulo, string logoPath, string marcaAguaPath, string? usuario = null);
    }
}

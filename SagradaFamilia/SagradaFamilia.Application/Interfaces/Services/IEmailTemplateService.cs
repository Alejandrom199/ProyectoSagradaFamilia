namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        Task<(string Asunto, string Cuerpo)> GenerarAsync(string codigo, IDictionary<string, string> variables);
    }
}

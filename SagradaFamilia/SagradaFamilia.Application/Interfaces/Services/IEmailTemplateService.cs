namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        string GenerarResetPassword(string nombreUsuario, string linkReset);
    }
}

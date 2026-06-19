using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;

namespace SagradaFamilia.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly ResendSettings _settings;
        private readonly IResend _resend;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<ResendSettings> settings, IResend resend, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _resend = resend;
            _logger = logger;
        }

        public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var mensaje = new EmailMessage
            {
                From = $"{_settings.FromName} <{_settings.FromEmail}>",
                Subject = asunto,
                HtmlBody = cuerpoHtml,
            };
            mensaje.To.Add(destinatario);

            _logger.LogInformation("Enviando email a {Destinatario} vía Resend — Asunto: {Asunto}", destinatario, asunto);

            await _resend.EmailSendAsync(mensaje);

            _logger.LogInformation("Email enviado exitosamente a {Destinatario}", destinatario);
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;
using System.Net;
using System.Net.Mail;

namespace SagradaFamilia.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            using var mensaje = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            using var cliente = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            _logger.LogInformation("Enviando email a {Destinatario} — Asunto: {Asunto}", destinatario, asunto);

            await cliente.SendMailAsync(mensaje);

            _logger.LogInformation("Email enviado exitosamente a {Destinatario}", destinatario);
        }
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;

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
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;
            mensaje.Body = new TextPart("html") { Text = cuerpoHtml };

            using var cliente = new SmtpClient();

            _logger.LogInformation("Conectando a SMTP {Host}:{Port}", _settings.Host, _settings.Port);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            var socketOptions = _settings.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
            await cliente.ConnectAsync(_settings.Host, _settings.Port, socketOptions, cts.Token);
            await cliente.AuthenticateAsync(_settings.Username, _settings.Password, cts.Token);

            _logger.LogInformation("Enviando email a {Destinatario} — Asunto: {Asunto}", destinatario, asunto);

            await cliente.SendAsync(mensaje, cts.Token);
            await cliente.DisconnectAsync(true, cts.Token);

            _logger.LogInformation("Email enviado exitosamente a {Destinatario}", destinatario);
        }
    }
}

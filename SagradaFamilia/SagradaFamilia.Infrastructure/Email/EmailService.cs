using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Settings;
using System.Net.Http.Json;
using System.Text.Json;

namespace SagradaFamilia.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly BrevoSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<BrevoSettings> settings, IHttpClientFactory httpClientFactory, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClientFactory.CreateClient("Brevo");
            _logger = logger;
        }

        public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var payload = new
            {
                sender = new { name = _settings.FromName, email = _settings.FromEmail },
                to = new[] { new { email = destinatario } },
                subject = asunto,
                htmlContent = cuerpoHtml
            };

            _logger.LogInformation("Enviando email a {Destinatario} vía Brevo — Asunto: {Asunto}", destinatario, asunto);

            var response = await _httpClient.PostAsJsonAsync("smtp/email", payload);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Brevo rechazó el email. Status: {Status} — Body: {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Error al enviar email vía Brevo: {response.StatusCode}");
            }

            _logger.LogInformation("Email enviado exitosamente a {Destinatario}", destinatario);
        }
    }
}

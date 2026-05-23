using SagradaFamilia.Application.Interfaces.Services;
using System.Reflection;

namespace SagradaFamilia.Infrastructure.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private static readonly Assembly _assembly = typeof(EmailTemplateService).Assembly;

        public string GenerarResetPassword(string nombreUsuario, string linkReset)
        {
            var html = LeerPlantilla("ResetPassword.html");
            return html
                .Replace("{{NOMBRE_USUARIO}}", nombreUsuario)
                .Replace("{{LINK_RESET}}", linkReset);
        }

        private static string LeerPlantilla(string nombreArchivo)
        {
            // El recurso embebido tiene el nombre: {Namespace}.Email.Templates.{archivo}
            var nombreRecurso = _assembly
                .GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(nombreArchivo, StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException($"Plantilla de email no encontrada: {nombreArchivo}");

            using var stream = _assembly.GetManifestResourceStream(nombreRecurso)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}

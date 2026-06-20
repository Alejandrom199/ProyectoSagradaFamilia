using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Interfaces.Repositories;

namespace SagradaFamilia.Infrastructure.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IEventoCorreoRepository _eventoRepository;

        public EmailTemplateService(IEventoCorreoRepository eventoRepository)
            => _eventoRepository = eventoRepository;

        public async Task<(string Asunto, string Cuerpo)> GenerarAsync(
            string codigoEvento, IDictionary<string, string> variables)
        {
            var evento = await _eventoRepository.ObtenerPorCodigoConPlantillaAsync(codigoEvento)
                ?? throw new InvalidOperationException(
                    $"No existe un evento de correo con código '{codigoEvento}'. Verificá que el seed se haya ejecutado.");

            if (evento.PlantillaCorreoId is null || evento.Plantilla is null)
                throw new InvalidOperationException(
                    $"El evento '{evento.Nombre}' no tiene ninguna plantilla de correo asignada.");

            if (!evento.Plantilla.Activo)
                throw new InvalidOperationException(
                    $"La plantilla asignada al evento '{evento.Nombre}' está marcada como inactiva.");

            var cuerpo = evento.Plantilla.Cuerpo;
            foreach (var (clave, valor) in variables)
                cuerpo = cuerpo.Replace($"{{{{{clave}}}}}", valor);

            return (evento.Plantilla.Asunto, cuerpo);
        }
    }
}

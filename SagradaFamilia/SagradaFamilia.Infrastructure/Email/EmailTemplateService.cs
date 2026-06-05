using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;

namespace SagradaFamilia.Infrastructure.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IPlantillaCorreoRepository _plantillaRepository;

        public EmailTemplateService(IPlantillaCorreoRepository plantillaRepository)
        {
            _plantillaRepository = plantillaRepository;
        }

        public async Task<(string Asunto, string Cuerpo)> GenerarAsync(
            string codigo, IDictionary<string, string> variables)
        {
            var plantilla = await _plantillaRepository.ObtenerPorCodigoAsync(codigo)
                ?? throw new InvalidOperationException(
                    $"No existe una plantilla activa con código '{codigo}'. Verificá que el seed se haya ejecutado.");

            var cuerpo = plantilla.Cuerpo;
            foreach (var (clave, valor) in variables)
                cuerpo = cuerpo.Replace($"{{{{{clave}}}}}", valor);

            return (plantilla.Asunto, cuerpo);
        }
    }
}

using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class ActualizarConsultaValidator : AbstractValidator<ConsultaDto.Actualizar>
    {
        public ActualizarConsultaValidator()
        {
            RuleFor(x => x.Motivo)
                .MaximumLength(500).WithMessage("El motivo no puede superar los 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Motivo));

            RuleFor(x => x.Diagnostico)
                .MaximumLength(500).WithMessage("El diagnóstico no puede superar los 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Diagnostico));

            RuleFor(x => x.Indicaciones)
                .MaximumLength(2000).WithMessage("Las indicaciones no pueden superar los 2000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Indicaciones));

            RuleFor(x => x.Evolucion)
                .MaximumLength(2000).WithMessage("La evolución no puede superar los 2000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Evolucion));
        }
    }
}

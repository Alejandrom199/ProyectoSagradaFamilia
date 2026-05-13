using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearCitaValidator : AbstractValidator<CitaDto.Create>
    {
        public CrearCitaValidator()
        {
            RuleFor(x => x.NinoId).GreaterThan(0).WithMessage("Debe seleccionar al paciente.");
            RuleFor(x => x.MedicoId).GreaterThan(0).WithMessage("Debe seleccionar al médico.");

            RuleFor(x => x.FechaHora)
                .NotEmpty().WithMessage("La fecha y hora son obligatorias.")
                .GreaterThan(DateTime.Now).WithMessage("No puede agendar una cita en el pasado.");

            RuleFor(x => x.Motivo)
                .MaximumLength(200).WithMessage("El motivo no puede superar los 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Motivo));
        }
    }

    public class ActualizarCitaValidator : AbstractValidator<CitaDto.Update>
    {
        public ActualizarCitaValidator()
        {
            RuleFor(x => x.MedicoId).GreaterThan(0).WithMessage("Debe seleccionar al médico.");
            RuleFor(x => x.FechaHora)
                .NotEmpty().WithMessage("La fecha y hora son obligatorias.")
                .GreaterThan(DateTime.Now).WithMessage("La cita no puede ser re-agendada en el pasado.");
            RuleFor(x => x.Motivo).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Motivo));
        }
    }
}
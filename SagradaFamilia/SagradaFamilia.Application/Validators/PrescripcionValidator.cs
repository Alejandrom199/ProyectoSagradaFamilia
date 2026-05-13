using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearPrescripcionValidator : AbstractValidator<PrescripcionDto.Create>
    {
        public CrearPrescripcionValidator()
        {
            RuleFor(x => x.NinoId).GreaterThan(0).WithMessage("Debe seleccionar al paciente.");


            RuleFor(x => x.DetalleMedicamentos)
                .NotEmpty().WithMessage("Debe ingresar al menos un medicamento en la receta.")
                .MaximumLength(1000).WithMessage("El detalle es demasiado extenso (máximo 1000 caracteres).");

            RuleFor(x => x.Indicaciones)
                .MaximumLength(1000).WithMessage("Las indicaciones no pueden superar los 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Indicaciones));
        }
    }

    public class ActualizarPrescripcionValidator : AbstractValidator<PrescripcionDto.Update>
    {
        public ActualizarPrescripcionValidator()
        {
            RuleFor(x => x.DetalleMedicamentos)
                .NotEmpty().WithMessage("Debe ingresar al menos un medicamento en la receta.")
                .MaximumLength(1000).WithMessage("El detalle es demasiado extenso.");

            RuleFor(x => x.Indicaciones)
                .MaximumLength(1000).WithMessage("Las indicaciones no pueden superar los 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Indicaciones));
        }
    }
}
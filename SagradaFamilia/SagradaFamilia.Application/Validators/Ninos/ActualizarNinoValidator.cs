using FluentValidation;
using SagradaFamilia.Application.DTOs.Ninos;

namespace SagradaFamilia.Application.Validators.Ninos
{
    public class ActualizarNinoValidator : AbstractValidator<ActualizarNinoRequest>
    {
        public ActualizarNinoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(100).WithMessage("El apellido no puede superar 100 caracteres.");

            RuleFor(x => x.FechaNacimiento)
                .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
                .LessThan(DateOnly.FromDateTime(DateTime.Today))
                    .WithMessage("La fecha de nacimiento no puede ser futura.")
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-6)))
                    .WithMessage("El sistema solo admite niños de hasta 5 años.");

            RuleFor(x => x.Sexo)
                .Must(s => s == 'M' || s == 'F')
                .WithMessage("El sexo debe ser 'M' o 'F'.");
        }
    }
}

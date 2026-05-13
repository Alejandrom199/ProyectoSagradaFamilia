using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearNinoValidator : AbstractValidator<NinoDto.Create>
    {
        public CrearNinoValidator()
        {
            RuleFor(x => x.PadreId).GreaterThan(0).WithMessage("El representante es obligatorio.");

            RuleFor(x => x.MedicoId).GreaterThan(0).WithMessage("El médico tratante es obligatorio.");

            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
            RuleFor(x => x.FechaNacimiento)
                .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
                .LessThan(DateOnly.FromDateTime(DateTime.Today)).WithMessage("La fecha de nacimiento no puede ser futura.")
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-6))).WithMessage("El sistema solo admite niños de hasta 5 años.");
            RuleFor(x => x.Sexo).Must(s => s == 'M' || s == 'F').WithMessage("El sexo debe ser 'M' o 'F'.");
        }
    }
}
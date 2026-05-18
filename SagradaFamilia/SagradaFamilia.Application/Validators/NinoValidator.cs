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
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-15)))
                .WithMessage("El sistema solo admite pacientes de hasta 14 años.");
            RuleFor(x => x.Sexo).Must(s => s == 'M' || s == 'F').WithMessage("El sexo debe ser 'M' o 'F'.");
        }
    }
}
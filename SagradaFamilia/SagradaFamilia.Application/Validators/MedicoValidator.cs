using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearMedicoValidator : AbstractValidator<MedicoDto.Create>
    {
        public CrearMedicoValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Especialidad).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Especialidad));
            RuleFor(x => x.Telefono).MaximumLength(15).When(x => !string.IsNullOrEmpty(x.Telefono));
        }
    }

    public class ActualizarMedicoValidator : AbstractValidator<MedicoDto.Update>
    {
        public ActualizarMedicoValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Especialidad).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Especialidad));
            RuleFor(x => x.Telefono).MaximumLength(15).When(x => !string.IsNullOrEmpty(x.Telefono));
        }
    }
}
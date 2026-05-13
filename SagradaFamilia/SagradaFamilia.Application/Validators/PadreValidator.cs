using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearPadreValidator : AbstractValidator<PadreDto.Create>
    {
        public CrearPadreValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

            RuleFor(x => x.MedicoId).GreaterThan(0).WithMessage("Debe asignar un médico tratante.");

            RuleFor(x => x.Telefono).MaximumLength(15).When(x => x.Telefono != null);
        }
    }
}
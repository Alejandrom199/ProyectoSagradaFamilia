using FluentValidation;
using SagradaFamilia.Application.DTOs.Auth;

namespace SagradaFamilia.Application.Validators.Auth
{
    public class CrearPadreValidator : AbstractValidator<CrearPadreRequest>
    {
        public CrearPadreValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(100).WithMessage("El apellido no puede superar 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El formato del email no es válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

            RuleFor(x => x.Telefono)
                .MaximumLength(15).WithMessage("El teléfono no puede superar 15 caracteres.")
                .When(x => x.Telefono is not null);
        }
    }
}

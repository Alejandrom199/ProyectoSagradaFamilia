using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearUsuarioValidator : AbstractValidator<UsuarioDto.Create>
    {
        public CrearUsuarioValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El formato del email no es válido.");

            RuleFor(x => x.RolId)
                .GreaterThan(0).WithMessage("Debe seleccionar un rol válido para el usuario.");
        }
    }

    public class ActualizarUsuarioValidator : AbstractValidator<UsuarioDto.Update>
    {
        public ActualizarUsuarioValidator()
        {
            RuleFor(x => x.RolId)
                .GreaterThan(0).WithMessage("Debe seleccionar un rol válido para el usuario.");
        }
    }
}
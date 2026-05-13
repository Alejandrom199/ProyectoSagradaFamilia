using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearCategoriaValidator : AbstractValidator<CategoriaDto.Create>
    {
        public CrearCategoriaValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede superar 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }

    public class ActualizarCategoriaValidator : AbstractValidator<CategoriaDto.Update>
    {
        public ActualizarCategoriaValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede superar 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }
    }
}
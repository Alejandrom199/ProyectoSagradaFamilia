using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class CrearAlimentoValidator : AbstractValidator<AlimentoDto.Create>
    {
        public CrearAlimentoValidator()
        {
            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("La categoría es obligatoria.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del alimento es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

            RuleFor(x => x.EdadMinimaMeses)
                .GreaterThanOrEqualTo(0).WithMessage("La edad mínima no puede ser negativa.");

            RuleFor(x => x.Recomendacion)
                .MaximumLength(500).WithMessage("La recomendación no puede superar 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Recomendacion));
        }
    }

    public class ActualizarAlimentoValidator : AbstractValidator<AlimentoDto.Update>
    {
        public ActualizarAlimentoValidator()
        {
            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("La categoría es obligatoria.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del alimento es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

            RuleFor(x => x.EdadMinimaMeses)
                .GreaterThanOrEqualTo(0).WithMessage("La edad mínima no puede ser negativa.");

            RuleFor(x => x.Recomendacion)
                .MaximumLength(500).WithMessage("La recomendación no puede superar 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Recomendacion));
        }
    }
}
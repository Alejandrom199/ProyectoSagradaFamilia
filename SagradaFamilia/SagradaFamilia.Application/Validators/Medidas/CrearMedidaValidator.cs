using FluentValidation;
using SagradaFamilia.Application.DTOs.Medidas;

namespace SagradaFamilia.Application.Validators.Medidas
{
    public class CrearMedidaValidator : AbstractValidator<CrearMedidaRequest>
    {
        public CrearMedidaValidator()
        {
            RuleFor(x => x.NinoId)
                .GreaterThan(0).WithMessage("El niño es obligatorio.");

            RuleFor(x => x.FechaMedicion)
                .NotEmpty().WithMessage("La fecha de medición es obligatoria.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                    .WithMessage("La fecha no puede ser futura.");

            RuleFor(x => x.Peso)
                .GreaterThan(0).WithMessage("El peso debe ser mayor a 0.")
                .LessThanOrEqualTo(30).WithMessage("El peso parece incorrecto.");

            RuleFor(x => x.Talla)
                .GreaterThan(0).WithMessage("La talla debe ser mayor a 0.")
                .LessThanOrEqualTo(130).WithMessage("La talla parece incorrecta.");
        }
    }
}

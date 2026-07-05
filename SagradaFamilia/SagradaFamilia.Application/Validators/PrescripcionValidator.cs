using FluentValidation;
using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Validators
{
    public class MedicamentoItemValidator : AbstractValidator<MedicamentoDto.Item>
    {
        public MedicamentoItemValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Debe indicar el nombre del medicamento.")
                .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");

            RuleFor(x => x.Presentacion)
                .MaximumLength(200).WithMessage("La presentación no puede superar los 200 caracteres.");

            RuleFor(x => x.Dosis)
                .NotEmpty().WithMessage("Debe indicar la dosis.")
                .MaximumLength(100).WithMessage("La dosis no puede superar los 100 caracteres.");

            RuleFor(x => x.Frecuencia)
                .NotEmpty().WithMessage("Debe indicar la frecuencia.")
                .MaximumLength(100).WithMessage("La frecuencia no puede superar los 100 caracteres.");

            RuleFor(x => x.ViaAdministracion)
                .NotEmpty().WithMessage("Debe indicar la vía de administración.")
                .MaximumLength(100).WithMessage("La vía de administración no puede superar los 100 caracteres.");

            RuleFor(x => x.Duracion)
                .MaximumLength(100).WithMessage("La duración no puede superar los 100 caracteres.");

            RuleFor(x => x.Cantidad)
                .MaximumLength(100).WithMessage("La cantidad no puede superar los 100 caracteres.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }
    }

    public class CrearPrescripcionValidator : AbstractValidator<PrescripcionDto.Create>
    {
        public CrearPrescripcionValidator()
        {
            RuleFor(x => x.ConsultaId)
                .GreaterThan(0).WithMessage("Debe seleccionar una consulta válida.");

            RuleFor(x => x.Medicamentos)
                .NotEmpty().WithMessage("Debe agregar al menos un medicamento a la receta.");

            RuleForEach(x => x.Medicamentos).SetValidator(new MedicamentoItemValidator());

            RuleFor(x => x.Indicaciones)
                .MaximumLength(2000).WithMessage("Las indicaciones no pueden superar los 2000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Indicaciones));
        }
    }

    public class ActualizarPrescripcionValidator : AbstractValidator<PrescripcionDto.Update>
    {
        public ActualizarPrescripcionValidator()
        {
            RuleFor(x => x.Medicamentos)
                .NotEmpty().WithMessage("Debe agregar al menos un medicamento a la receta.");

            RuleForEach(x => x.Medicamentos).SetValidator(new MedicamentoItemValidator());

            RuleFor(x => x.Indicaciones)
                .MaximumLength(2000).WithMessage("Las indicaciones no pueden superar los 2000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Indicaciones));
        }
    }
}

using FluentValidation;
using PhoneHub.Core.DTOs;

namespace PhoneHub.Services.Validators
{
    public class CrearProductoDtoValidator : AbstractValidator<ProductDto>
    {
        public CrearProductoDtoValidator()
        {
            RuleFor(x => x.Id)
                .Equal(0).When(x => x.Id != 0)
                .WithMessage("El ID debe ser 0 o no enviarse para la creación de un nuevo producto.");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("La marca es obligatoria.")
                .MaximumLength(50).WithMessage("La marca no puede exceder los 50 caracteres.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("El modelo es obligatorio.")
                .MaximumLength(100).WithMessage("El modelo no puede exceder los 100 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
        }
    }
}

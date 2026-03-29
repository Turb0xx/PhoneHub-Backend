using FluentValidation;
using PhoneHub.Core.DTOs;

namespace PhoneHub.Services.Validators
{
    public class SaleRequestDtoValidator : AbstractValidator<SaleRequestDto>
    {
        public SaleRequestDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("El ID del producto es obligatorio y debe ser mayor que cero.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("El ID del usuario es obligatorio y debe ser mayor que cero.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
        }
    }
}

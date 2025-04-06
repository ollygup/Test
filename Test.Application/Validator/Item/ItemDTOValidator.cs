using FluentValidation;
using Test.Application.DTO.Item;

namespace Test.Application.Validator.Item
{
    public class ItemDTOValidator : AbstractValidator<ItemDTO>
    {
        public ItemDTOValidator()
        {
            RuleFor(x => x.PartnerItemRef)
                .NotEmpty().WithMessage("PartnerItemRef is required.")
                .MaximumLength(50).WithMessage("PartnerItemRef cannot exceed 50 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Qty)
                .InclusiveBetween(1, 5).WithMessage("Quantity must be between 1 and 5.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("UnitPrice must be a positive value.");
        }
    }
}

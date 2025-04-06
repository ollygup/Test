using FluentValidation;
using Test.Application.DTO.Transaction;
using Test.Application.Validator.Item;

namespace Test.Application.Validator.Transaction
{
    public class TransactionInputDTOValidator : AbstractValidator<TransactionInputDTO>
    {
        public TransactionInputDTOValidator()
        {
            // PartnerKey validation
            RuleFor(x => x.PartnerKey)
                .NotEmpty().WithMessage("PartnerKey is required.")
                .MaximumLength(50).WithMessage("PartnerKey cannot exceed 50 characters.");

            // PartnerRefNo validation
            RuleFor(x => x.PartnerRefNo)
                .NotEmpty().WithMessage("PartnerRefNo is required.")
                .MaximumLength(50).WithMessage("PartnerRefNo cannot exceed 50 characters.");

            // PartnerPassword validation
            RuleFor(x => x.PartnerPassword)
                .NotEmpty().WithMessage("PartnerPassword is required.")
                .MaximumLength(50).WithMessage("PartnerPassword cannot exceed 50 characters.");

            // TotalAmount validation
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("TotalAmount must be a positive value.");

            // Timestamp validation (ISO 8601 format)
            RuleFor(x => x.Timestamp)
                .NotEmpty().WithMessage("Timestamp is required.")
                .Matches(@"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+([+-][0-2]\d:[0-5]\d|Z)")
                .WithMessage("Timestamp must be in ISO 8601 format (yyyy-MM-ddTHH:mm:ss.sssZ).");

            // Sig validation
            RuleFor(x => x.Sig)
                .NotEmpty().WithMessage("Sig is required.");

            // Item validations (If exists)
            RuleForEach(x => x.Items).SetValidator(new ItemDTOValidator());

            // Validate total amount in items
            RuleFor(x => x)
                .Must(input =>
                {
                    if (input.Items == null || !input.Items.Any())
                    {
                        return true;
                    }

                    var calculatedTotal = input.Items.Sum(item => item.Qty * item.UnitPrice);
                    return calculatedTotal == input.TotalAmount;
                })
                .WithMessage("Invalid Total Amount.");
        }
    }
}

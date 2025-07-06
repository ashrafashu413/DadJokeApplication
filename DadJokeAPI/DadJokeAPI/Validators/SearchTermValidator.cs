using FluentValidation;

namespace Validators
{
    public class SearchTermValidator : AbstractValidator<string>
    {
        public SearchTermValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Search term cannot be empty.")
                .MinimumLength(2).WithMessage("Search term must be at least 2 characters.");
        }
    }
}

using BookstoreSimulator.Contracts;
using FluentValidation;

namespace BookstoreSimulator.Infra.Validation
{
    public class SingUpUserRequestValidator : AbstractValidator<SingUpUserRequest>
    {
        public SingUpUserRequestValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty().WithMessage("Your first name cannot be empty");
            RuleFor(user => user.LastName).NotEmpty().WithMessage("Your last name cannot be empty");

            RuleFor(user => user.Email)
                    .NotEmpty().WithMessage("Your email address cannot be empty")
                    .EmailAddress().WithMessage("Your email address shold be valid");

            RuleFor(user => user.Password)
                    .NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
        }
    }
}

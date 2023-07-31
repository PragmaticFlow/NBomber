using BookstoreSimulator.Contracts;
using FluentValidation;

namespace BookstoreSimulator.Infra.Validation
{
    public class LoginUserRequestValidator: AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Your email cannot be empty")
                .EmailAddress().WithMessage("Your email address shold be valid");

            RuleFor(user => user.Password).NotEmpty().WithMessage("Your password cannot be empty");
        }
    }
}

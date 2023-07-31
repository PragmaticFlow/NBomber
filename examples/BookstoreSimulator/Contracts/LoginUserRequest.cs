using FluentValidation;

namespace BookstoreSimulator.Contracts
{
    public class LoginUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

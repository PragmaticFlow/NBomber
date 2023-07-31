using FluentValidation;

namespace BookstoreSimulator.Contracts
{
    public class SingUpUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

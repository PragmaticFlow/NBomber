namespace Demo.HTTP.SimpleBookstore.Contracts
{
    public class UserSingup
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

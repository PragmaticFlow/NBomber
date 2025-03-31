namespace WebAppSimulator.Contracts
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public User ToUser()
        {
            return new User
            {
                Id = 0,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Age = this.Age
            };
        }
    }
}

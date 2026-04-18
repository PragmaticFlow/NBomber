namespace HttpApiSimulator.Contracts;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}

public class UpdateUserReq
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public User ToUser(int id = 0)
    {
        return new User
        {
            Id = id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Age = this.Age
        };
    }
}

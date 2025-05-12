using Bogus;
using GraphQLServer.Contracts;

namespace GraphQLServer.Data;

public static class DbInitializer
{
    public static void Initialize(UserDbContext context)
    {
        if (context.Users.Any())
            return;

        var roleAdmin = new Role { Name = "Admin" };
        var roleGuest = new Role { Name = "Guest" };

        var testUsers = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Country, f => f.Address.Country())
            .RuleFor(u => u.Age, f => f.Random.Int(1, 100))
            .RuleFor(u => u.RoleId, f => f.Random.Int(1, 2));

        testUsers.GenerateBetween(100, 100);

        context.Roles.AddRange(roleAdmin, roleGuest);
        context.Users.AddRange(testUsers.GenerateBetween(100, 1000));

        context.SaveChanges();
    }
}

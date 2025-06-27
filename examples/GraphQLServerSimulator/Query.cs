using GraphQLServer.Contracts;
using GraphQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace GraphQLServer;

public class Query
{
    [UseFiltering]
    public IQueryable<User> GetUsers(UserDbContext dbContext) => dbContext.Users.Include(u => u.Role);
}

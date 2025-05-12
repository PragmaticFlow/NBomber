using GraphQLServer;
using GraphQLServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"))
    .AddLogging(Console.WriteLine);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering();

var app = builder.Build();

app.CreateDbIfNotExists();

app.MapGraphQL();

app.Run();

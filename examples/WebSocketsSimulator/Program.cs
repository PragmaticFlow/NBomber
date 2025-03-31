var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseWebSockets();

app.MapControllers();

app.Run();

using WebAppSimulator.Infra.DAL;

namespace WebAppSimulator
{
    public class SQLiteSettings
    {
        public string ConnectionString { get; set; }
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string ServerName { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication();

            var dbUse = builder.Configuration.GetValue("DbUse", "");

            if (dbUse == "SQLite")
            {
                var settings = builder.Configuration.GetSection("SQLiteSettings").Get<SQLiteSettings>();
                var rep = new SQLiteDBRepository(settings);
                builder.Services.AddSingleton<IUserRepository>(rep);
            }
            else if (dbUse == "Redis")
            {
                var settings = builder.Configuration.GetSection("RedisSetings").Get<RedisSettings>();
                var rep = new RedisRepository(settings);
                builder.Services.AddSingleton<IUserRepository>(rep);
            }

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();
            app.UseWebSockets();

            app.Run();
        }
    }
}

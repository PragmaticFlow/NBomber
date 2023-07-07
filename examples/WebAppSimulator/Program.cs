using System.Data.SQLite;
using WebAppSimulator.Infra.DAL;

namespace WebAppSimulator
{
    public class SQLiteSettings
    {
        public string ConnectionString { get; set; }
        public int ConnectionCount { get; set; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddSwaggerGen();

            var settings = builder.Configuration.GetSection("SQLiteSettings").Get<SQLiteSettings>();
            var rep = new SQLiteDBRepository(settings);
            builder.Services.AddSingleton<IUserRepository>(rep);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseHttpsRedirection();

            if (app.Environment.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = context =>
                    {
                        context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                        context.Context.Response.Headers.Add("Expires", "-1");
                    }
                });

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            app.Run();
        }
    }
}

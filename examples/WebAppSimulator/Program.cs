using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAppSimulator.Infra.Bookstore.DAL;
using WebAppSimulator.Infra.DAL;

namespace WebAppSimulator
{
    public class JwtSetings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
    public class BookstoreSettings
    {
        public string ConnectionString { get; set; }
    }
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
            builder.Services.AddRazorPages();

            var jwtSetings = builder.Configuration.GetSection("JWT").Get<JwtSetings>();

            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = "ASP.NET Core 7.0 Web API"
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
            });
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetings.Issuer,
                    ValidAudience = jwtSetings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetings.Key))
                };
            });
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
            else if (dbUse == "PostgreSQL")
            {
                var settings = builder.Configuration.GetSection("BookstoreSettings").Get<BookstoreSettings>();
                var rep = new BookstoreUserRepository(settings);
                builder.Services.AddSingleton(rep);
                builder.Services.AddSingleton(jwtSetings);

                var db = new DB(settings);
                db.CreateTables();
            }
            else
                throw new Exception("The base for the test is not specified in the file appsettings.json");

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

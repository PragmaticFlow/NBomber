using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.ListenLocalhost(5000);
                })
                .UseStartup<Startup>();
        }
    }
}

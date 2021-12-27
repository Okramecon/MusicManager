using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicManager.Data;

namespace MusicManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost= CreateHostBuilder(args).Build();
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<AppDbContext>();
                InitialDataFiller filler = new InitialDataFiller(context);
                filler.FillDatabase();
            }
            webHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

using DemoConsoleApplication;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DemoConsoleApplication
{
    public class Program
    {
#if DEBUG
        private const string DEFAULT_ENVIRONMENT = "Development";
#else
        const string DEFAULT_ENVIRONMENT = "Production";        
#endif

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            host.Start();
            var program = host.Services.GetService(typeof(IApplication)) as IApplication;
            await program.Run();
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT") ?? DEFAULT_ENVIRONMENT)
                .ConfigureServices(Startup.ConfigureServices)
                .UseConsoleLifetime();
        }
    }
}

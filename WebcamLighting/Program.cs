using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebcamLighting.Elgato;

namespace WebcamLighting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IWebcamMonitor, WindowsWebcamMonitor>();
                    services.AddSingleton<ILightsManager, ElgatoKeyLightsManager>();
                    services.AddHostedService<ServiceWorker>();
                })
                .UseWindowsService();
    }
}

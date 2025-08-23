using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebcamLighting.Elgato;
using System;
using System.Reflection;

namespace WebcamLighting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    if (OperatingSystem.IsWindows())
                    {
                        services.AddSingleton<IWebcamMonitor, WindowsWebcamMonitor>();
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        // use reflection to avoid compile-time reference to linux-only type which triggers
                        // platform compatibility warnings/errors when building on other platforms
                        var linuxType = Assembly.GetExecutingAssembly().GetType("WebcamLighting.LinuxWebcamMonitor");
                        if (linuxType != null)
                        {
                            services.AddSingleton(typeof(IWebcamMonitor), linuxType);
                        }
                        else
                        {
                            services.AddSingleton<IWebcamMonitor, NoopWebcamMonitor>();
                        }
                    }
                    else
                    {
                        // Fallback to Noop for unknown platforms
                        services.AddSingleton<IWebcamMonitor, NoopWebcamMonitor>();
                    }

                    services.AddSingleton<ILightsManager, ElgatoKeyLightsManager>();
                    services.AddHostedService<ServiceWorker>();
                });

            if (OperatingSystem.IsWindows())
            {
                builder = builder.UseWindowsService();
            }

            return builder;
        }
    }
}

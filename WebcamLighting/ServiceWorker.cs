using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebcamLighting
{
    public class ServiceWorker : BackgroundService
    {
        private readonly IWebcamMonitor myWebcamMonitor;
        private readonly ILightsManager myLightsManager;
        private readonly ILogger<ServiceWorker> myLogger;

        public ServiceWorker(IWebcamMonitor webcamMonitor, ILightsManager lightsManager, ILogger<ServiceWorker> logger)
        {
            myWebcamMonitor = webcamMonitor;
            myLightsManager = lightsManager;
            myLogger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            myLogger.LogInformation("Started service worker");

            var wasInUse = myWebcamMonitor.CurrentlyInUse();
            myLogger.LogInformation(wasInUse ? "Initially camera was used" : "Initially camera was not used");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);

                if (myWebcamMonitor.CurrentlyInUse())
                {
                    if (wasInUse)
                    {
                        myLogger.LogTrace("Webcam still in use");
                        continue;
                    }

                    myLogger.LogInformation("Webcam is now used");

                    myLightsManager.Lights.AsParallel().ForAll(x => x.On());
                    wasInUse = true;
                }
                else
                {
                    if (wasInUse)
                    {
                        myLogger.LogInformation("Webcam is no longer used");
                        myLightsManager.Lights.AsParallel().ForAll(x => x.Off());
                    }
                    else
                    {
                        myLogger.LogTrace("Webcam still not used");
                    }

                    wasInUse = false;
                }
            }
        }
    }
}

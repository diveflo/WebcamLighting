using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace WebcamLighting.Elgato
{
    public class ElgatoKeyLightsManager : ILightsManager
    {
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger<ElgatoKeyLightsManager> myLogger;

        public IList<ILightController> Lights
        {
            get; private set;
        }

        public ElgatoKeyLightsManager(ILoggerFactory loggerFactory)
        {
            myLoggerFactory = loggerFactory;
            myLogger = myLoggerFactory.CreateLogger<ElgatoKeyLightsManager>();

            Lights = FindElgatoKeyLights().ToList();

            Task.Run(() => ContinouslySearchForLights(new CancellationToken()));
        }

        private IEnumerable<ILightController> FindElgatoKeyLights()
        {
            myLogger.LogInformation("Starting Bonjour search for Elgato Keylights");
            var elgatoLights = ZeroconfResolver.ResolveAsync("_elg._tcp.local.", TimeSpan.FromSeconds(5)).Result;
            myLogger.LogInformation($"Found {elgatoLights.Count} Elgato Keylights.");

            foreach (var light in elgatoLights)
            {
                yield return new ElgatoKeyLightController(light.IPAddress, light.Id, myLoggerFactory.CreateLogger<ElgatoKeyLightController>());
            }
        }

        private void ContinouslySearchForLights(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));

                Lights = FindElgatoKeyLights().ToList();
            }
        }
    }
}

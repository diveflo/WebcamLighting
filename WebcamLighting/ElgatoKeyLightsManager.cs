using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Zeroconf;

namespace WebcamLighting
{
    public class ElgatoKeyLightsManager : ILightsManager
    {
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger<ElgatoKeyLightsManager> myLogger;

        public IList<ILightController> Lights { get; }

        public ElgatoKeyLightsManager(ILoggerFactory loggerFactory)
        {
            myLoggerFactory = loggerFactory;
            myLogger = myLoggerFactory.CreateLogger<ElgatoKeyLightsManager>();

            Lights = FindElgatoKeyLights().ToList();
        }

        private IEnumerable<ILightController> FindElgatoKeyLights()
        {
            myLogger.LogInformation("Starting Bonjour search for Elgato Keylights");
            var elgatoLights = ZeroconfResolver.ResolveAsync("_elg._tcp.local.").Result;
            myLogger.LogInformation($"Found {elgatoLights.Count} Elgato Keylights.");

            foreach (var light in elgatoLights)
            {
                yield return new ElgatoKeyLightController(light.IPAddress, light.Id, myLoggerFactory.CreateLogger<ElgatoKeyLightController>());
            }
        }
    }
}

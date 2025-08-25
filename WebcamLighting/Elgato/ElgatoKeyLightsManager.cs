using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace WebcamLighting.Elgato
{
    /// <summary>
    /// Manages Elgato Key Lights, discovers them via Bonjour, and keeps the list updated.
    /// </summary>
    public class ElgatoKeyLightsManager : ILightsManager
    {
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger<ElgatoKeyLightsManager> myLogger;
        private readonly object myLightsLock = new object();
        private IList<ILightController> myLights;
        private CancellationTokenSource myCts;

        /// <summary>
        /// Gets the current list of discovered lights (thread-safe).
        /// </summary>
        public IList<ILightController> Lights
        {
            get
            {
                lock (myLightsLock)
                {
                    return myLights;
                }
            }
            private set
            {
                lock (myLightsLock)
                {
                    myLights = value;
                }
            }
        }

        /// <summary>
        /// Constructs a new ElgatoKeyLightsManager and starts background discovery.
        /// </summary>
        public ElgatoKeyLightsManager(ILoggerFactory loggerFactory)
        {
            myLoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            myLogger = myLoggerFactory.CreateLogger<ElgatoKeyLightsManager>();
            myLights = new List<ILightController>();
            myCts = new CancellationTokenSource();

            Task.Run(() => DiscoverAndUpdateLightsAsync(myCts.Token));
        }

        /// <summary>
        /// Discovers Elgato Key Lights asynchronously and updates the Lights property.
        /// </summary>
        private async Task DiscoverAndUpdateLightsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    myLogger.LogInformation("Starting Bonjour search for Elgato Keylights");
                    var elgatoLights = await ZeroconfResolver.ResolveAsync("_elg._tcp.local.", TimeSpan.FromSeconds(5));
                    myLogger.LogInformation($"Found {elgatoLights.Count} Elgato Keylights.");
                    var discovered = elgatoLights
                        .Select(light => (ILightController)new ElgatoKeyLightController(light.IPAddress, light.Id, myLoggerFactory.CreateLogger<ElgatoKeyLightController>()))
                        .ToList();
                    Lights = discovered;
                }
                catch (Exception ex)
                {
                    myLogger.LogError(ex, "Error discovering Elgato Keylights");
                }
                
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }

        /// <summary>
        /// Disposes the manager and stops background discovery.
        /// </summary>
        public void Dispose()
        {
            myCts?.Cancel();
            myCts?.Dispose();
        }
    }
}

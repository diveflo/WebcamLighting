using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebcamLighting.Elgato
{
    /// <summary>
    /// Controls an Elgato Key Light via its REST API.
    /// </summary>
    public class ElgatoKeyLightController : ILightController
    {
        private readonly string myLightIP;
        private readonly ILogger<ElgatoKeyLightController> myLogger;

        /// <summary>
        /// Gets the unique identifier for the light.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Constructs a new ElgatoKeyLightController.
        /// </summary>
        public ElgatoKeyLightController(string ip, string id, ILogger<ElgatoKeyLightController> logger)
        {
            myLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            myLightIP = ip ?? throw new ArgumentNullException(nameof(ip));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        /// Turns the light on asynchronously.
        /// </summary>
        public async void On()
        {
            try
            {
                var currentState = await GetLightStateAsync();
                if (currentState.On == 1)
                {
                    myLogger.LogInformation($"Light {Id} already turned on");
                    return;
                }

                var requestedState = currentState;
                requestedState.On = 1;

                myLogger.LogInformation($"Turning on light {Id}");
                await SetLightStateAsync(requestedState);
            }
            catch (Exception ex)
            {
                myLogger.LogError(ex, $"Error turning on light {Id}");
            }
        }

        /// <summary>
        /// Turns the light off asynchronously.
        /// </summary>
        public async void Off()
        {
            try
            {
                var currentState = await GetLightStateAsync();
                if (currentState.On == 0)
                {
                    myLogger.LogInformation($"Light {Id} already turned off");
                    return;
                }

                var requestedState = currentState;
                requestedState.On = 0;

                myLogger.LogInformation($"Turning off light {Id}");
                await SetLightStateAsync(requestedState);
            }
            catch (Exception ex)
            {
                myLogger.LogError(ex, $"Error turning off light {Id}");
            }
        }

        /// <summary>
        /// Sets the brightness asynchronously.
        /// </summary>
        public async void SetBrightness(int brightness)
        {
            try
            {
                var currentState = await GetLightStateAsync();
                if (currentState.Brightness == brightness)
                {
                    myLogger.LogInformation($"Light {Id} already set to {brightness}% brightness");
                    return;
                }

                var requestedState = currentState;
                requestedState.Brightness = brightness;

                myLogger.LogInformation($"Setting light {Id} to {brightness}% brightness");
                await SetLightStateAsync(requestedState);
            }
            catch (Exception ex)
            {
                myLogger.LogError(ex, $"Error setting brightness for light {Id}");
            }
        }

        /// <summary>
        /// Sets the color temperature asynchronously.
        /// </summary>
        public async void SetTemperature(int temperature)
        {
            try
            {
                var currentState = await GetLightStateAsync();
                if (currentState.Temperature == temperature)
                {
                    myLogger.LogInformation($"Light {Id} already set to {temperature}K color temperature");
                    return;
                }

                var requestedState = currentState;
                requestedState.Temperature = temperature;

                myLogger.LogInformation($"Setting light {Id} to {temperature}K color temperature");
                await SetLightStateAsync(requestedState);
            }
            catch (Exception ex)
            {
                myLogger.LogError(ex, $"Error setting temperature for light {Id}");
            }
        }

        /// <summary>
        /// Gets the current state of the light asynchronously.
        /// </summary>
        private async Task<ElgatoLight> GetLightStateAsync()
        {
            try
            {
                using var webClient = new HttpClient();
                var response = await webClient.GetAsync($"http://{myLightIP}:9123/elgato/lights");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonSerializer.Deserialize<ElgatoREST>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var lightState = parsedResponse?.Lights?.FirstOrDefault();
                if (lightState == null)
                {
                    throw new InvalidDataException($"No light state returned for light {Id}");
                }
                myLogger.LogInformation($"Got state for light {Id}: {lightState}");
                return lightState;
            }
            catch (Exception ex)
            {
                myLogger.LogError(ex, $"Error getting state for light {Id}");
                throw;
            }
        }

        /// <summary>
        /// Sets the state of the light asynchronously.
        /// </summary>
        private async Task SetLightStateAsync(ElgatoLight requiredState)
        {
            try
            {
                var requestUri = $"http://{myLightIP}:9123/elgato/lights";
                using var httpClient = new HttpClient();
                var content = JsonSerializer.Serialize(new ElgatoREST { NumberOfLights = 1, Lights = new[] { requiredState } });
                var response = await httpClient.PutAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                myLogger.LogError(ex, $"Error setting state for light {Id}");
                throw;
            }
        }
    }
}

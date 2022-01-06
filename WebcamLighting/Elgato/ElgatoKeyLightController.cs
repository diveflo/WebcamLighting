using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebcamLighting.Elgato
{
    public class ElgatoKeyLightController : ILightController
    {
        private readonly string myLightIP;
        private readonly ILogger<ElgatoKeyLightController> myLogger;

        public bool IsOn => GetLightState().Result.On == 1;

        public string Id
        {
            get;
        }

        public ElgatoKeyLightController(string ip, string id, ILogger<ElgatoKeyLightController> logger)
        {
            myLogger = logger;
            myLightIP = ip;
            Id = id;
        }

        public void On()
        {
            var currentState = GetLightState().Result;
            if (currentState.On == 1)
            {
                myLogger.LogInformation($"Light {Id} already turned on");
                return;
            }

            var requestedState = currentState;
            requestedState.On = 1;

            myLogger.LogInformation($"Turning on light {Id}");
            SetLightStateAsync(requestedState).Wait(1000);
        }

        public void Off()
        {
            var currentState = GetLightState().Result;
            if (currentState.On == 0)
            {
                myLogger.LogInformation($"Light {Id} already turned off");
                return;
            }

            var requestedState = currentState;
            requestedState.On = 0;

            myLogger.LogInformation($"Turning off light {Id}");
            SetLightStateAsync(requestedState).Wait(1000);
        }

        public void SetBrightness(int brightness)
        {
            var currentState = GetLightState().Result;
            if (currentState.Brightness == brightness)
            {
                myLogger.LogInformation($"Light {Id} already set to {brightness}% brightness");
                return;
            }

            var requestedState = currentState;
            requestedState.Brightness = brightness;

            myLogger.LogInformation($"Setting light {Id} to {brightness}% brightness");
            SetLightStateAsync(requestedState).Wait(1000);
        }

        public void SetTemperature(int temperature)
        {
            var currentState = GetLightState().Result;
            if (currentState.Temperature == temperature)
            {
                myLogger.LogInformation($"Light {Id} already set to {temperature}K color temperature");
                return;
            }

            var requestedState = currentState;
            requestedState.Temperature = temperature;

            myLogger.LogInformation($"Setting light {Id} to {temperature}K color temperature");
            SetLightStateAsync(requestedState).Wait(1000);
        }

        private async Task<ElgatoLight> GetLightState()
        {
            using var webClient = new HttpClient();

            var response = await webClient.GetAsync($"http://{myLightIP}:9123/elgato/lights");
            response.EnsureSuccessStatusCode();
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var responseStreamReader = new StreamReader(responseStream);

            var parsedResponse = JsonSerializer.Deserialize<ElgatoREST>(responseStreamReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var lightState = parsedResponse.Lights.First();

            myLogger.LogInformation($"Got state for light {Id}: {lightState}");
            return parsedResponse.Lights.First();
        }

        private async Task SetLightStateAsync(ElgatoLight requiredState)
        {
            var requestUri = $"http://{myLightIP}:9123/elgato/lights";

            var httpClient = new HttpClient();
            var content = JsonSerializer.Serialize(new ElgatoREST { NumberOfLights = 1, Lights = new[] { requiredState } });
            var response = await httpClient.PutAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
    }
}

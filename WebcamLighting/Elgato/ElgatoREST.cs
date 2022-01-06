using System.Text.Json.Serialization;

namespace WebcamLighting.Elgato
{
    public class ElgatoREST
    {
        [JsonPropertyName("numberOfLights")]
        public int NumberOfLights { get; set; }

        [JsonPropertyName("lights")]
        public ElgatoLight[] Lights { get; set; }
    }

}

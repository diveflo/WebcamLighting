using System.Text.Json.Serialization;

namespace WebcamLighting.Elgato
{
    public class ElgatoLight
    {
        [JsonPropertyName("on")]
        public int On { get; set; }

        [JsonPropertyName("brightness")]
        public int Brightness { get; set; }

        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        public override string ToString()
        {
            var powerStateDescription = On == 1 ? "On/" : "Off/";
            return powerStateDescription + $"{Brightness}% brigthness/{Temperature}K color temperature";
        }
    }

}

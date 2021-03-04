namespace WebcamLighting
{
    public interface ILightController
    {
        public string Id { get; }
        public void On();

        public void Off();

        public void SetBrightness(int brightness);

        public void SetTemperature(int temperature);
    }
}

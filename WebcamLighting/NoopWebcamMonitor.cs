namespace WebcamLighting
{
    public class NoopWebcamMonitor : IWebcamMonitor
    {
        public bool CurrentlyInUse() => false;
    }
}

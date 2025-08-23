using Xunit;

namespace WebcamLighting.Tests
{
    public class WebcamMonitorTests
    {
        [Fact]
        public void NoopWebcamMonitor_ShouldNeverDetectWebcam()
        {
            var monitor = new NoopWebcamMonitor();
            Assert.False(monitor.CurrentlyInUse());
        }
    }
}
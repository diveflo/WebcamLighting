using Xunit;
using Moq;

namespace WebcamLighting.Tests
{
    public class ServiceWorkerTests
    {
        [Fact]
        public void ServiceWorker_CanBeConstructed()
        {
            var mockManager = new Mock<ILightsManager>().Object;
            var mockMonitor = new Mock<IWebcamMonitor>().Object;
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ServiceWorker>>().Object;
            var worker = new ServiceWorker(mockMonitor, mockManager, mockLogger);
            Assert.IsType<ServiceWorker>(worker);
        }
    }
}
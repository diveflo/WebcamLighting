using Xunit;
using Moq;

namespace WebcamLighting.Tests
{
    public class ILightControllerTests
    {
        [Fact]
        public void On_ShouldActivateLight()
        {
            var mockController = new Mock<ILightController>();
            mockController.Setup(c => c.On()).Verifiable();
            mockController.Object.On();
            mockController.Verify(c => c.On(), Times.Once);
        }

        [Fact]
        public void Off_ShouldDeactivateLight()
        {
            var mockController = new Mock<ILightController>();
            mockController.Setup(c => c.Off()).Verifiable();
            mockController.Object.Off();
            mockController.Verify(c => c.Off(), Times.Once);
        }
    }
}
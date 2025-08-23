using Xunit;
using Moq;
using System.Collections.Generic;

namespace WebcamLighting.Tests
{
    public class ILightsManagerTests
    {
        [Fact]
        public void LightsManager_LightsProperty_ShouldContainControllers()
        {
            var mockController = new Mock<ILightController>().Object;
            var mockManager = new Mock<ILightsManager>();
            mockManager.Setup(m => m.Lights).Returns(new List<ILightController> { mockController });
            Assert.Contains(mockController, mockManager.Object.Lights);
        }

        [Fact]
        public void LightsManager_LightsProperty_ShouldNotContainController()
        {
            var mockController = new Mock<ILightController>().Object;
            var mockManager = new Mock<ILightsManager>();
            mockManager.Setup(m => m.Lights).Returns(new List<ILightController>());
            Assert.DoesNotContain(mockController, mockManager.Object.Lights);
        }
    }
}
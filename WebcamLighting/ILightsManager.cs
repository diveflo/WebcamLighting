using System.Collections.Generic;

namespace WebcamLighting
{
    public interface ILightsManager
    {
        public IList<ILightController> Lights { get; }
    }
}

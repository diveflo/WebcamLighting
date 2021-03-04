using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace WebcamLighting
{
    [SupportedOSPlatform("windows")]
    public class WindowsWebcamMonitor : IWebcamMonitor
    {
        private readonly RegistryKey myRegistryKey;

        public WindowsWebcamMonitor()
        {
            myRegistryKey = Registry.CurrentUser
                .OpenSubKey("SOFTWARE")
                .OpenSubKey("Microsoft")
                .OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion")
                .OpenSubKey("CapabilityAccessManager")
                .OpenSubKey("ConsentStore")
                .OpenSubKey("webcam")
                .OpenSubKey("NonPackaged");
        }

        private IEnumerable<WebcamUsingProcess> GetAllProcessesEverUsingWebcam()
        {
            var processesEverUsedWebcam = myRegistryKey.GetSubKeyNames();

            foreach (var process in processesEverUsedWebcam)
            {
                yield return new WebcamUsingProcess(myRegistryKey.OpenSubKey(process));
            }
        }

        public bool CurrentlyInUse()
        {
            return GetAllProcessesEverUsingWebcam().Any(x => x.IsCurrentlyUsingWebcam);
        }
    }
}

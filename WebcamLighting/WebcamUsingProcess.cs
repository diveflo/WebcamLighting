using Microsoft.Win32;
using System.Runtime.Versioning;

namespace WebcamLighting
{
    [SupportedOSPlatform("windows")]
    public class WebcamUsingProcess
    {
        private readonly RegistryKey myProcessRegistryKey;

        public string ExecutableName => ExecutableFullName.Split(@"\")[^1];

        public string ExecutableFullName => myProcessRegistryKey.Name.Replace(@"#", @"\");

        public bool IsCurrentlyUsingWebcam => myProcessRegistryKey.GetValue("LastUsedTimeStop").ToString().Equals("0");

        public WebcamUsingProcess(RegistryKey key)
        {
            myProcessRegistryKey = key;
        }
    }
}

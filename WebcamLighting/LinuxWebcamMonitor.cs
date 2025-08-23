using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;

namespace WebcamLighting
{
    [SupportedOSPlatform("linux")]
    public class LinuxWebcamMonitor : IWebcamMonitor
    {
        private readonly ILogger<LinuxWebcamMonitor> myLogger;

        public LinuxWebcamMonitor(ILogger<LinuxWebcamMonitor> logger)
        {
            myLogger = logger;
        }

        public bool CurrentlyInUse()
        {
            try
            {
                var procDir = new DirectoryInfo("/proc");

                foreach (var pidDir in procDir.EnumerateDirectories().Where(d => d.Name.All(char.IsDigit)))
                {
                    var fdDirPath = Path.Combine(pidDir.FullName, "fd");
                    if (!Directory.Exists(fdDirPath))
                        continue;

                    var fdDir = new DirectoryInfo(fdDirPath);

                    foreach (var fdEntry in fdDir.EnumerateFileSystemInfos())
                    {
                        // FileSystemInfo.LinkTarget is available on .NET Core / .NET 5+ and returns the symlink target on Linux.
                        string linkTarget = null;
                        try
                        {
                            linkTarget = fdEntry.LinkTarget;
                        }
                        catch (Exception innerEx)
                        {
                            // Fall back to FullName if LinkTarget isn't available or permitted
                            myLogger?.LogDebug(innerEx, "Could not get LinkTarget for {Path}", fdEntry.FullName);
                            linkTarget = fdEntry.FullName;
                        }

                        if (!string.IsNullOrEmpty(linkTarget) && linkTarget.Contains("/dev/video", StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                myLogger?.LogWarning(ex, "UnauthorizedAccess while scanning /proc for webcam usage. Some entries may be inaccessible.");
            }
            catch (DirectoryNotFoundException ex)
            {
                myLogger?.LogWarning(ex, "/proc not found - not a Linux-like system or /proc not mounted.");
            }
            catch (Exception ex)
            {
                myLogger?.LogError(ex, "Unexpected error while checking webcam usage.");
            }

            return false;
        }
    }
}

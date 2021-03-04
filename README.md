# Webcam Lighting

Automatically turn your Elgato Keylight on when your PC's webcam is in use. And off when it's not.

## Install

Download the executable from GitHub and either run it directly or install as a (Windows) service.
To install as a service, you can use `sc.exe`, e.g., like this:

```shell
sc.exe create "Webcam Lights" binPath= "C:\Users\youraccount\webcamlighting\webcamlighting.exe" obj= ".\youraccount" password= "YourPassword"
```

*Hint*: The spaces after `binPath`, `obj` and `password` are required!

## Build

The tool is based on `.NET 5` and can therefore be easily build in Visual Studio or your favorite shell using:

```shell
dotnet build
```

## Extend for non-Windows and non-Elgato lights

Currently, this tool only works for Windows-based systems and Elgato Keylights. However, this can be easily extended thanks to `.NET 5`.
To support e.g. Linux-based systems, only a new class implementing the `IWebcamMonitor` interface needs to be added and setup for dependency inject in `Program.cs`.
Additionally, `.UseWindowsService()` would have to be replaced by `.UseSystemd()`.

Non-Elgato lights would require two new classes implementing the `ILightController` and `ILightManager` interfaces and be wired into the DI setup in `Program.cs`

## Thanks

[@fleckmart](https://github.com/flecmart) figured out that there is a Windows registry key that can be used to determine if (and by whom) the system's webcam is currently being used (see [fleckmart/windows_webcam_monitor](https://github.com/flecmart/windows_webcam_monitor)).

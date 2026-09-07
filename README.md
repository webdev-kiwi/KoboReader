# KoboReader

KoboReader is a small Windows desktop application that provides a focused window for the Kobo web library. It is built with WPF and hosts the Kobo website using Microsoft Edge WebView2.

## Features

- Opens the Kobo library at `https://www.kobo.com/nz/en/library/books`.
- Uses a native Windows desktop window instead of a separate browser tab.
- Opens links that request a new browser window in the system default browser.
- Stores the WebView2 user data under `%LOCALAPPDATA%\KoboReader`.

## Requirements

- Windows.
- The .NET 10 SDK.
- Microsoft Edge WebView2 Runtime. The application includes the WebView2 .NET package, but the WebView2 Runtime must be installed on the computer where the application runs.
- Internet access and a Kobo account for accessing the Kobo library.

The project targets `net10.0-windows` and uses WPF, so it is not intended to run on Linux or macOS.

## Getting started

Clone the repository and open the solution or project in Visual Studio:

```powershell
git clone <repository-url>
cd KoboReader
start .\KoboReader.slnx
```

Alternatively, use the .NET CLI from the repository directory.

Restore dependencies:

```powershell
dotnet restore .\KoboReader.csproj
```

Build a Debug version:

```powershell
dotnet build .\KoboReader.csproj --configuration Debug
```

Run the application:

```powershell
dotnet run --project .\KoboReader.csproj
```

After the application starts, sign in to Kobo if required. The application loads the configured Kobo library URL in the embedded browser.

## Publishing

### Framework-dependent publish

This produces a smaller deployment that requires the matching .NET Desktop Runtime to be installed on the target machine:

```powershell
dotnet publish .\KoboReader.csproj `
  --configuration Release `
  --framework net10.0-windows `
  --runtime win-x64 `
  --self-contained false `
  --output .\publish\framework-dependent
```

Run the published application from the output directory:

```powershell
.\publish\framework-dependent\KoboReader.exe
```

### Self-contained publish

This includes the .NET runtime and is suitable for machines that do not already have the .NET Desktop Runtime installed. It still requires the WebView2 Runtime:

```powershell
dotnet publish .\KoboReader.csproj `
  --configuration Release `
  --framework net10.0-windows `
  --runtime win-x64 `
  --self-contained true `
  --output .\publish\win-x64
```

For a smaller self-contained deployment, you can enable single-file publishing:

```powershell
dotnet publish .\KoboReader.csproj `
  --configuration Release `
  --framework net10.0-windows `
  --runtime win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  --output .\publish\win-x64-single-file
```

The commands above target 64-bit Windows. To publish for another Windows architecture, replace `win-x64` with a supported runtime identifier such as `win-arm64` or `win-x86`.

## Distribution notes

When distributing a published build:

1. Distribute the complete contents of the publish directory, unless using a verified single-file deployment.
2. Ensure the target machine has the Microsoft Edge WebView2 Runtime installed.
3. Ensure the target machine can reach Kobo's website.
4. Do not distribute user data from `%LOCALAPPDATA%\KoboReader`; it may contain local browser state.

The application does not package Kobo credentials. Authentication is handled by the Kobo website inside WebView2.

## Project structure

| Path | Description |
| --- | --- |
| `KoboReader.csproj` | WPF project configuration and WebView2 package reference. |
| `App.xaml` / `App.xaml.cs` | Application resources and startup configuration. |
| `MainWindow.xaml` | Main window and embedded WebView2 control. |
| `MainWindow.xaml.cs` | WebView2 initialization and navigation behavior. |
| `Icons/favicon.ico` | Application and window icon. |

## Configuration

The initial Kobo library URL is defined in `MainWindow.xaml.cs`. To use a different Kobo regional library, update the `Browser.Source` URI and rebuild the application.

The WebView2 profile is stored in the per-user directory:

```text
%LOCALAPPDATA%\KoboReader
```

Delete that directory to clear the application's local WebView2 profile and sign-in state.

## License

No license file is currently included in this repository. Add a `LICENSE` file before publishing the project under an open-source license.

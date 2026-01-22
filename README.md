# TokenExtractor

Small console utility that captures an authentication cookie (token) from Microsoft Edge. It can either start a fresh Edge WebDriver session or connect to an existing Edge profile running with remote debugging enabled, then waits for a specific cookie and saves it to disk.

## Features
- Two modes: launch a new Edge instance or reuse your existing profile via remote debugging.
- Watches for a configurable cookie name and optional success URL fragment.
- Saves the captured token to a `txt` file in the working directory.
- Colored console prompts for clearer steps.

## Requirements
- Windows with Microsoft Edge installed.
- .NET 10 SDK (project targets `net10.0-windows7.0`).
- Internet access to download the matching Edge WebDriver (handled by `WebDriverManager`).

## Build and run
```bash
dotnet restore
dotnet run --project TokenExtractor/TokenExtractor.csproj
# or
dotnet publish TokenExtractor/TokenExtractor.csproj -c Release -r win-x64 --self-contained false
```

## Usage
When started, choose a mode:

### 1) Start a NEW browser session
- The app downloads the correct Edge driver and opens Edge at the login page.
- Complete your login; the app watches for the target cookie and saves it as `<tokenName>.txt`.

### 2) Use an EXISTING browser session
- Close all Edge windows first.
- The app restarts Edge with remote debugging on the chosen port and your existing user data directory.
- After the login flow completes, the target cookie is captured and written to `<tokenName>.txt`.

## Configuration prompts
Defaults shown at startup (press Enter to keep):
- Edge path: `C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`
- Debug port: `9222` (only for existing session mode)
- Login URL: `https://cfg.aramarklabor.com/workforcesoftware/air`
- Success URL part: `/air` (waits until the URL contains this fragment)
- Token name: `wfm_token` (cookie name to extract)

## Tips and troubleshooting
- Mode 2 (existing session) should be run as a standard user so Edge can access your profile.
- If Edge refuses to start with your profile, Windows Ransomware Protection / Controlled Folder Access may be blocking access to the Edge user data folder. Allow the app executable through that protection and retry.
- If the browser closes before capture, re-run and complete the login again.

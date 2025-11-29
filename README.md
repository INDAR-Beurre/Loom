# Loom (WPF) — Native port of the Loom Browser UI

This workspace contains a WPF port of the original Loom browser UI (the HTML file provided by you). The project reproduces the core look-and-feel in XAML and provides:

- Native WPF layout and UI in `MainWindow.xaml` and Views (HomeView, SearchView)
- Multiple tabs, an address / search bar, and a right-side assistant panel placeholder (AI simulation)
- WebView2 integration for site navigation (site views open inside a WebView2 instance)
- A small publish script and an Inno Setup script to create a Windows installer

Extras:
- build-installer.ps1 — wrapper that publishes the app and attempts to compile the Inno Setup script (requires iscc.exe on PATH)

Quick start (Windows PowerShell):

```powershell
# build
dotnet build .\Loom.csproj

# run
dotnet run --project .\Loom.csproj

# publish for packaging
.\scripts\publish.ps1 -Configuration Release -Runtime win-x64
```

Creating an installer:

1. Publish the app (see publish.ps1).
2. Install Inno Setup and open `installer\LoomInstaller.iss`.
3. Replace the `{#PublishFolder}` preprocessor variable with the published folder path (or set via preprocessor), then compile.

Notes:
- WebView2 runtime should be installed on the system for the browser content to work.
- This repo contains a near-faithful visual XAML port — there are more improvements possible (AI connector, history/bookmarks persistence, more advanced tab management).

Building the installer (CI / locally)
=====================================

This project includes a GitHub Actions workflow to build a Windows installer (Inno Setup) on a Windows runner and upload the produced installer as an artifact.

How it works
- The workflow `.github/workflows/build-installer.yml` runs on `workflow_dispatch` or when you push a `v*` tag.
- It publishes a Release `dotnet publish` output then installs Inno Setup via Chocolatey on the runner and calls `scripts/build-installer.ps1`.
- When complete it uploads the produced EXE as an artifact for download.

Run it locally
- Install Inno Setup on your local machine (https://jrsoftware.org/isdl.php) and ensure `iscc.exe` is on your PATH.
- Publish the app:
```powershell
.
cd 'C:\path\to\Loom'
.\scripts\publish.ps1 -Configuration Release -Runtime win-x64
```
- Build the installer:
```powershell
.
cd 'C:\path\to\Loom'
.
powershell -ExecutionPolicy Bypass -File .\scripts\build-installer.ps1 -Configuration Release -Runtime win-x64
```

Run on GitHub Actions
- Push a tag like `v1.0.0` or run the workflow manually via Actions → Build Windows Installer.

Notes
- If you prefer another installer system (NSIS, WiX) or want code signing, let me know and I can add CI steps for those as well.

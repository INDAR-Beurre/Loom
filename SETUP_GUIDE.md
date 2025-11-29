# Loom Browser - XAML WebView Setup Guide

## Overview
This guide shows you how to use the Loom Browser HTML with a XAML WebView control in a .NET application.

## The Problem That Was Fixed
**Original Issue:** When clicking buttons like "YouTube", the browser tried to navigate directly to `youtube.com` instead of searching for it on Google.

**Solution:** Modified the button click handlers to perform Google searches:
```
Before: onClick={() => navigate(link.url)}
After: onClick={() => navigate(`https://www.google.com/search?q=${encodeURIComponent(link.title)})`)}
```

Now when you tap "YouTube", it will:
1. Go to Google.com
2. Search for "YouTube"
3. Display the search results

## Files Provided

1. **index-fixed.html** - The corrected browser HTML with search fix
2. **BrowserView.xaml** - The XAML UI definition for WebView2
3. **BrowserView.xaml.cs** - The C# code-behind

## Setup Instructions

### Option 1: WinUI 3 / Windows App SDK (Recommended)

#### Prerequisites
- Visual Studio 2022
- Windows App SDK installed
- WebView2 Runtime installed on your system

#### Steps

1. **Create a new WinUI 3 project:**
   ```
   File → New → Project → "WinUI 3 in Desktop"
   ```

2. **Install WebView2 NuGet package:**
   ```
   Tools → NuGet Package Manager → Package Manager Console
   Install-Package Microsoft.Web.WebView2
   ```

3. **Add the XAML namespace:**
   In `BrowserView.xaml`, add this namespace declaration:
   ```xaml
   xmlns:webview2="using:Microsoft.Web.WebView2.UI.Xaml"
   ```

4. **Replace your MainWindow.xaml** with the provided `BrowserView.xaml`

5. **Replace your MainWindow.xaml.cs** with the provided `BrowserView.xaml.cs`

6. **Copy the HTML files** to your project:
   - Place `index-fixed.html` in your project root (where the .csproj file is)
   - Mark it as "Copy if newer" in Properties

7. **Build and Run:**
   - Press F5 or Build → Build Solution

### Option 2: UWP (Universal Windows Platform)

#### Prerequisites
- Visual Studio 2022
- Windows 10/11 SDK
- WebView control is built-in

#### Steps

1. **Create a new UWP project:**
   ```
   File → New → Project → "Blank App (Universal Windows)"
   ```

2. **Replace MainPage.xaml** with BrowserView.xaml (adjust namespace for UWP)

3. **Update BrowserView.xaml** for UWP:
   ```xaml
   <Page
       x:Class="LoomBrowser.MainPage"
       xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
       
       <Grid>
           <WebView x:Name="BrowserWebView" 
                    NavigationCompleted="BrowserWebView_NavigationCompleted"/>
       </Grid>
   </Page>
   ```

4. **Update the C# code-behind** for UWP:
   ```csharp
   private void InitializeWebView()
   {
       string htmlPath = Path.Combine(AppContext.BaseDirectory, "index-fixed.html");
       if (File.Exists(htmlPath))
       {
           BrowserWebView.NavigateToString(File.ReadAllText(htmlPath));
       }
   }
   ```

## How It Works

### Option 3: WPF (dotnet-cli)

If you want to run the included source directly using the dotnet CLI (WPF + WebView2):

Prerequisites
- .NET 6 SDK (or later with net6.0-windows support)
- WebView2 Runtime installed on the system

Quick steps
1. Restore and build
```powershell
dotnet restore
dotnet build c:\Users\alexa\OneDrive\Dokumente\Loom\Loom.csproj
```

2. Run with the CLI (or run the produced exe)
```powershell
dotnet run --project c:\Users\alexa\OneDrive\Dokumente\Loom\Loom.csproj
# or run the executable directly
Start-Process -FilePath "bin\Debug\net6.0-windows\Loom.exe"
```

Note: `index-fixed.html` is included in the project and will be copied to the build output folder. If you modify it, set "Copy to Output Directory" to "Copy if newer".

### Packaging & Creating an Installer (Windows)

You can publish the app and create an installer using the included scripts and Inno Setup.

1. Build the release publish
```powershell
.\scripts\publish.ps1 -Configuration Release -Runtime win-x64
```

2. The publish output will be in `./publish/win-x64/Release`.

3. Download and install Inno Setup (https://jrsoftware.org/isinfo.php).

4. Open `installer\LoomInstaller.iss` in Inno Setup and replace the preprocessor variable `{#PublishFolder}` with the absolute path to the published folder (or use the Inno preprocessor), then compile it to produce LoomSetup.exe.

Notes:
- The installer script is minimal and intended as a starting point — you can add shortcuts, uninstaller options, registry entries or signing as needed.
- The Windows machine needs the WebView2 runtime; include instructions to bundle or instruct users to install the WebView2 runtime.

### Search Flow:
1. User types "YouTube" in search box OR clicks YouTube button
2. Button click now triggers: `navigate('https://www.google.com/search?q=YouTube')`
3. Browser navigates to Google with the search query
4. Google displays search results for YouTube

### Direct Navigation:
- The normal search input still works as before
- Type any URL or search query → it performs a search via DuckDuckGo API

## Testing

1. **Test Search Buttons:**
   - Click "YouTube" → Should show Google search results for YouTube
   - Click "GitHub" → Should show Google search results for GitHub
   - Click "Reddit" → Should show Google search results for Reddit
   - Click "Wikipedia" → Should show Google search results for Wikipedia

2. **Test Search Bar:**
   - Type "react hooks" → Should show DuckDuckGo results
   - Type "github.com" → Should navigate to GitHub

## Troubleshooting

### WebView2 not found error
- Download WebView2 Runtime from: https://developer.microsoft.com/en-us/microsoft-edge/webview2/
- Or install: `winget install Microsoft.WebView2Runtime`

### HTML file not found
- Ensure `index-fixed.html` is in the same directory as the executable
- Check Properties → Copy to Output Directory is set to "Copy if newer"

### Script errors in WebView
- Check browser console (Developer Tools)
- Ensure all CDN links (Tailwind, React, etc.) are accessible

## Customization

### To modify the quick links:
In `index-fixed.html`, find the COMMON_SITES array:
```javascript
const COMMON_SITES = [
    { title: "YouTube", url: "youtube.com", icon: "📺" },
    { title: "GitHub", url: "github.com", icon: "🐙" },
    // Add more here...
];
```

### To change search engine:
Replace `google.com/search` with your preferred engine:
- Bing: `bing.com/search?q=`
- DuckDuckGo: `duckduckgo.com/?q=`
- Ecosia: `ecosia.org/search?q=`

## Additional Resources

- WebView2 Documentation: https://learn.microsoft.com/en-us/microsoft-edge/webview2/
- WinUI 3 Documentation: https://learn.microsoft.com/en-us/windows/apps/winui/winui3/
- React Documentation: https://react.dev/

## Support

If you encounter issues:
1. Check the Debug Output window in Visual Studio
2. Open Developer Tools (F12 in WebView2)
3. Verify all file paths are correct
4. Ensure WebView2 Runtime is installed

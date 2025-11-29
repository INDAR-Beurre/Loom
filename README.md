<div align="center">

<img src="https://agi-prod-file-upload-public-main-use1.s3.amazonaws.com/9dead6af-a560-48ac-bafd-cc6cfe579700" alt="Loom Browser Logo" width="180"/>

# 🌐 Loom Browser

### _A Lightweight, AI-Powered Native Browser for Windows_

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/UI-WPF-orange.svg)](https://github.com/INDAR-Beurre/Loom)
[![Stars](https://img.shields.io/github/stars/INDAR-Beurre/Loom?style=social)](https://github.com/INDAR-Beurre/Loom/stargazers)

[Features](#-features) • [Quick Start](#-quick-start) • [Installation](#-installation) • [Documentation](#-documentation)

</div>

---

## ✨ Features

<table>
<tr>
<td width="50%">

### 🤖 **AI-Powered Assistant**
- Built-in AI integration for smart browsing
- Real-time assistance panel
- Intelligent search suggestions
- Context-aware recommendations

</td>
<td width="50%">

### ⚡ **Lightning Fast & Lightweight**
- Minimal memory footprint
- Native WPF performance
- Instant startup time
- Optimized rendering engine

</td>
</tr>
<tr>
<td width="50%">

### 🎨 **Beautiful Design**
- Modern, sleek interface
- Clean and intuitive UI/UX
- Smooth animations
- Aesthetic visual elements

</td>
<td width="50%">

### 🔧 **Developer Friendly**
- Open source & customizable
- WebView2 integration
- Easy to extend
- Well-documented codebase

</td>
</tr>
</table>

---

## 🚀 Quick Start

### Prerequisites

```bash
✓ Windows 10/11
✓ .NET 6.0 SDK or higher
✓ WebView2 Runtime (usually pre-installed)
```

### Build & Run

```powershell
# 📦 Clone the repository
git clone https://github.com/INDAR-Beurre/Loom.git
cd Loom

# 🔨 Build the project
dotnet build .\Loom.csproj

# 🚀 Run the browser
dotnet run --project .\Loom.csproj
```

---

## 📦 Installation

### Option 1: Build from Source

1. **Publish the application:**
   ```powershell
   .\scripts\publish.ps1 -Configuration Release -Runtime win-x64
   ```

2. **Create installer with Inno Setup:**
   - Install [Inno Setup](https://jrsoftware.org/isinfo.php)
   - Open `installer\LoomInstaller.iss`
   - Update the `{#PublishFolder}` path
   - Compile the installer

### Option 2: Automated Build Script

```powershell
# 🎯 One-command build & package (requires iscc.exe in PATH)
.\build-installer.ps1
```

---

## 🏗️ Architecture

### Project Structure

```
Loom/
├── 📁 Views/
│   ├── HomeView.xaml        # Home screen UI
│   └── SearchView.xaml      # Search interface
├── 📁 scripts/
│   ├── publish.ps1          # Build script
│   └── build-installer.ps1  # Installer builder
├── 📁 installer/
│   └── LoomInstaller.iss    # Inno Setup config
├── 📄 MainWindow.xaml       # Main browser window
└── 📄 Loom.csproj           # Project file
```

### Tech Stack

- **🎨 UI Framework:** WPF (Windows Presentation Foundation)
- **🌐 Web Engine:** Microsoft Edge WebView2
- **💻 Language:** C# / XAML
- **🤖 AI Integration:** Extensible assistant panel
- **📦 Packaging:** Inno Setup for Windows installers

---

## 🎯 Core Features

### 🗂️ **Multi-Tab Browsing**
Full-featured tab management with smooth transitions and efficient memory handling.

### 🔍 **Smart Address Bar**
Intelligent search and URL bar with autocomplete and suggestions.

### 🤖 **AI Assistant Panel**
Integrated AI helper on the right side for:
- Quick answers
- Content summarization
- Smart recommendations
- Productivity enhancements

### 🌐 **WebView2 Integration**
Powered by Microsoft Edge's Chromium engine for:
- Modern web standards support
- High performance rendering
- Secure browsing environment
- Full compatibility

---

## 📖 Documentation

### Configuration

The browser can be customized through the XAML files:

- **MainWindow.xaml** - Main window layout and structure
- **HomeView.xaml** - Customize the home screen
- **SearchView.xaml** - Modify search behavior and UI

### Extending the AI Features

The AI assistant panel is designed to be extensible. Connect your preferred AI backend by:

1. Implementing the assistant interface
2. Connecting to your AI service (OpenAI, local models, etc.)
3. Updating the panel UI with responses

### WebView2 Runtime

> **📌 Important:** WebView2 Runtime must be installed on the target system. Most Windows 10/11 systems have it pre-installed.

[Download WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)

---

## 🛣️ Roadmap

- [ ] 🔐 Enhanced privacy features
- [ ] 📚 Bookmarks & history persistence
- [ ] 🎨 Customizable themes
- [ ] 🔌 Extension support
- [ ] 🌙 Dark mode optimization
- [ ] ⚡ Performance improvements
- [ ] 🤖 Advanced AI integrations
- [ ] 📱 Cross-platform support

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. 🍴 Fork the repository
2. 🌿 Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. 💾 Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. 📤 Push to the branch (`git push origin feature/AmazingFeature`)
5. 🔀 Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 💬 Support

Have questions or need help?

- 🐛 [Report bugs](https://github.com/INDAR-Beurre/Loom/issues)
- 💡 [Request features](https://github.com/INDAR-Beurre/Loom/issues)
- ⭐ Star this repo if you find it useful!

---

<div align="center">

### 🌟 Built with passion for a better browsing experience

**Made by [INDAR-Beurre](https://github.com/INDAR-Beurre)**

[![GitHub followers](https://img.shields.io/github/followers/INDAR-Beurre?style=social)](https://github.com/INDAR-Beurre)

</div>
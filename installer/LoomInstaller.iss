[Setup]
AppName=Loom
AppVersion=0.1.0
DefaultDirName={pf}\Loom
DefaultGroupName=Loom
OutputBaseFilename=LoomSetup
Compression=lzma
SolidCompression=yes

[Files]
Source: "{#PublishFolder}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Loom"; Filename: "{app}\Loom.exe"

[Registry]
Root: HKCU; Subkey: "Software\Loom"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"

; replace {#PublishFolder} before running Inno Setup or use the compiler's preprocessor to set the path

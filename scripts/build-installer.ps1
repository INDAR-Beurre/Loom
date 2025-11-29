param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

Write-Host "Publishing Loom (config=$Configuration, runtime=$Runtime)"
# Call the existing publish script (relative to this scripts folder)
& "$PSScriptRoot\publish.ps1" -Configuration $Configuration -Runtime $Runtime

$publishPath = Join-Path -Path (Get-Location) -ChildPath ("publish\$Runtime\$Configuration")
if (-not (Test-Path $publishPath)) { Write-Error "Publish folder not found: $publishPath"; exit 1 }

Write-Host "Publish folder: $publishPath"

# Find Inno Setup compiler
$iscc = Get-Command -Name iscc.exe -ErrorAction SilentlyContinue
if ($iscc) {
    $issPath = Join-Path -Path (Get-Location) -ChildPath "installer\LoomInstaller.iss"
    Write-Host "Compiling installer using Inno Setup: $issPath"
    $isccPath = $iscc.Path
    & "$isccPath" "/DPublishFolder=$publishPath" "$issPath"
    exit 0
}

# Fallback: try using IExpress (built into Windows) to create a self-extracting installer if Inno isn't available
$iexpress = Join-Path $env:SystemRoot 'System32\iexpress.exe'
if (-not (Test-Path $iexpress)) {
    Write-Warning "Inno Setup compiler 'iscc.exe' not found and IExpress not available. Please install Inno Setup or run this script on a machine with Inno or IExpress."
    exit 0
}

Write-Host "Inno Setup not found. Falling back to IExpress (iexpress.exe) to build an SFX installer."

# Create a temporary SED file describing the package
$timestamp = Get-Date -Format 'yyyyMMddHHmmss'
$sedFile = Join-Path $env:TEMP ("loom_iexpress_$timestamp.sed")
$outExe = Join-Path (Get-Location) ("LoomInstaller_$timestamp.exe")

Write-Host "Assembling IExpress SED: $sedFile -> $outExe"

# Build file list entries (relative to publishPath)
$fileEntries = Get-ChildItem -Path $publishPath -Recurse -File | ForEach-Object { ($_).FullName }

$filesSection = ""
foreach ($f in $fileEntries) {
    $filesSection += "$f"
    $filesSection += "`r`n"
}

$headerLines = @(
    "[Version]",
    "Class=IEXPRESS",
    "SEDVersion=3",
    "",
    "[Options]",
    "PackagePurpose=InstallApp",
    "ShowInstallProgramWindow=1",
    "HideExtractAnimation=0",
    "UseLongFileName=1",
    "OverwriteMode=2",
    "CAB_Resv=0",
    "RebootMode=I",
    "InstallPrompt=",
    "DisplayLicense=",
    "FinishMessage=Installation complete.",
    "TargetName=$outExe",
    "FriendlyName=Loom",
    'AppLaunched="Loom.exe"',
    "PostInstallCmd=",
    "",
    "[Files]"
)

$sedContent = ($headerLines -join "`r`n") + "`r`n" + $filesSection + "`r`n[SourceFiles]`r`n" + "SourceFiles0=$publishPath`r`n; End of SED"

Set-Content -Path $sedFile -Value $sedContent -Encoding ASCII

& $iexpress /N /Q /M $sedFile

if (Test-Path $outExe) { Write-Host "IExpress package created: $outExe" } else { Write-Warning "IExpress failed to produce an installer. You can install Inno Setup to build a proper installer from the publish folder: $publishPath" }

$issPath = Join-Path -Path (Get-Location) -ChildPath "installer\LoomInstaller.iss"
Write-Host "Compiling installer using Inno Setup: $issPath"

# If Inno Setup is available now, attempt to compile the .iss script as well
$iscc = Get-Command -Name iscc.exe -ErrorAction SilentlyContinue
if ($iscc) {
    Write-Host "Found Inno Setup (iscc.exe) at: $($iscc.Path). Running Inno to build the MSI/EXE..."
    $isccPath = $iscc.Path
    & "$isccPath" "/DPublishFolder=$publishPath" "$issPath"
} else {
    Write-Host "Inno Setup not found; skipping Inno compile. Only IExpress package was produced (if successful)."
}

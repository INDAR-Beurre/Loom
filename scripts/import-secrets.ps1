# Interactive helper: import Supabase and Groq API keys into the Loom settings.json file (stored under %APPDATA%\Loom)
# This script will NOT commit your secrets to source control — it writes them to the per-user AppData folder.
# Usage: run this script locally in PowerShell and paste your keys when prompted.

$base = Join-Path $env:APPDATA 'Loom'
if (-not (Test-Path $base)) { New-Item -ItemType Directory -Path $base | Out-Null }
$settingsPath = Join-Path $base 'settings.json'

# Load existing settings, if any
$settings = @{}
if (Test-Path $settingsPath) {
    try { $settings = Get-Content $settingsPath -Raw | ConvertFrom-Json -ErrorAction Stop } catch { $settings = @{} }
}

function Prompt-Secure([string]$label, [bool]$mask = $true) {
    if ($mask) {
        $pw = Read-Host -AsSecureString "$label (input will be hidden)"
        return [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($pw))
    } else {
        return Read-Host $label
    }
}

Write-Host "This tool will save Supabase and Groq credentials to: $settingsPath" -ForegroundColor Yellow
Write-Host "Your secrets will only be stored locally on this machine (in AppData) and will NOT be added to source control." -ForegroundColor Green

$confirm = Read-Host "Continue and enter keys now? (y/N)"
if ($confirm -ne 'y' -and $confirm -ne 'Y') { Write-Host 'Aborted by user.'; exit 1 }

# Prompt for Supabase URL
$supUrl = Read-Host 'Supabase project URL (e.g. https://abcxyz.supabase.co) [leave blank to keep current]'
if (-not [string]::IsNullOrWhiteSpace($supUrl)) { $settings.SupabaseUrl = $supUrl }

# Prompt for Supabase ANON key
$supAnon = Prompt-Secure 'Supabase anon key (paste here)'
if (-not [string]::IsNullOrWhiteSpace($supAnon)) { $settings.SupabaseAnonKey = $supAnon }

# Prompt for Groq API key
$groq = Prompt-Secure 'Groq API key (paste here)'
if (-not [string]::IsNullOrWhiteSpace($groq)) { $settings.GroqApiKey = $groq }

# Ensure other fields remain if present
if (-not $settings.GroqModel) { $settings.GroqModel = 'llama3-8b-8192' }
if (-not $settings.SupabaseAccessToken) { $settings.SupabaseAccessToken = '' }
if (-not $settings.SupabaseUserId) { $settings.SupabaseUserId = '' }

# Write out
try {
    $json = $settings | ConvertTo-Json -Depth 4
    $json | Out-File -FilePath $settingsPath -Encoding UTF8
    Write-Host "Saved settings to $settingsPath" -ForegroundColor Green
} catch {
    Write-Host "Failed to write settings: $_" -ForegroundColor Red
    exit 1
}

Write-Host "Done. Start the Loom app and it will pick up these keys from your user settings." -ForegroundColor Cyan

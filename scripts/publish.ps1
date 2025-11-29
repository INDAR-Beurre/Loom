param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

Write-Host "Publishing Loom (config=$Configuration, runtime=$Runtime)"
dotnet restore
dotnet publish .\Loom.csproj -c $Configuration -r $Runtime --self-contained false -o .\publish\$Runtime\$Configuration

Write-Host "Publish output: .\publish\$Runtime\$Configuration"

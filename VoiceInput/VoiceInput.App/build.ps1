# Build.ps1 - Automates the build process for VoiceInput

param (
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputDir = "publish"
)

Write-Host "Building VoiceInput Application..."

# Verify dotnet CLI is available
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Error "dotnet CLI not found. Please install .NET SDK."
    exit 1
}

# Restore, build, and publish
& dotnet restore "..\VoiceInput.sln"
& dotnet publish "..\VoiceInput.sln" -c $Configuration -r $Runtime --self-contained -o $OutputDir

Write-Host "Build completed. Published to $OutputDir."
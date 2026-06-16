param (
    [string]$SourcePath = "..\upstream_pkhex",
    [string]$DestPath = "..\src"
)

$coreSource = Join-Path $SourcePath "PKHeX.Core"
$coreDest = Join-Path $DestPath "Roto.Core"

Write-Host "Starting renaming and integration process..."

if (Test-Path $coreDest) {
    Remove-Item $coreDest -Recurse -Force
}

# Remove old incorrect copies
if (Test-Path "..\src\PKHeX.Core") {
    Remove-Item "..\src\PKHeX.Core" -Recurse -Force
}
if (Test-Path "..\src\RotoDex.Engine") {
    Remove-Item "..\src\RotoDex.Engine" -Recurse -Force
}
if (Test-Path "..\src\RotoDex.Core") {
    # Only remove if it was mistakenly created as a copy of PKHeX
    Remove-Item "..\src\RotoDex.Core" -Recurse -Force
}

Write-Host "Copying PKHeX.Core to src\Roto.Core..."
Copy-Item -Path $coreSource -Destination $coreDest -Recurse

Write-Host "Renaming csproj file..."
Rename-Item -Path (Join-Path $coreDest "PKHeX.Core.csproj") -NewName "Roto.Core.csproj"

Write-Host "Replacing namespaces in source files..."
$files = Get-ChildItem -Path $coreDest -Include *.cs, *.csproj -Recurse
foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    if ($null -ne $content) {
        $newContent = $content -replace 'PKHeX\.Core', 'Roto.Core'
        if ($content -cne $newContent) {
            Set-Content -Path $f.FullName -Value $newContent -NoNewline
        }
    }
}

Write-Host "Integration and renaming to Roto.Core complete."

param(
    [string]$Root = (Split-Path $PSScriptRoot -Parent)
)

Write-Host "Root: $Root"

# ─── Step 1: Clean bin/obj ───────────────────────────────────────────────────
Write-Host "`n[1/6] Cleaning bin/obj folders..."
Get-ChildItem -Path $Root -Include bin,obj -Recurse -Directory |
    Where-Object { $_.FullName -notlike "*\.git*" -and $_.FullName -notlike "*\legacy\*" } |
    ForEach-Object { Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue }

# ─── Step 2: Rename csproj files inside PKHeX.* folders ─────────────────────
Write-Host "`n[2/6] Renaming csproj files..."
$renames = @(
    @{ From = "src\PKHeX.Drawing\PKHeX.Drawing.csproj";                    To = "src\PKHeX.Drawing\RotoDex.Drawing.csproj" },
    @{ From = "src\PKHeX.Drawing.Misc\PKHeX.Drawing.Misc.csproj";          To = "src\PKHeX.Drawing.Misc\RotoDex.Drawing.Misc.csproj" },
    @{ From = "src\PKHeX.Drawing.PokeSprite\PKHeX.Drawing.PokeSprite.csproj"; To = "src\PKHeX.Drawing.PokeSprite\RotoDex.Drawing.PokeSprite.csproj" },
    @{ From = "src\RotoDex.Desktop\Settings\PKHeXSettings.cs";              To = "src\RotoDex.Desktop\Settings\RotoDexSettings.cs" }
)
foreach ($r in $renames) {
    $src  = Join-Path $Root $r.From
    $dest = Join-Path $Root $r.To
    if (Test-Path $src) {
        Rename-Item $src $dest -Force
        Write-Host "  Renamed: $($r.From) -> $($r.To)"
    } else {
        Write-Host "  SKIP (not found): $($r.From)"
    }
}

# ─── Step 3: Text replacements in source files ───────────────────────────────
Write-Host "`n[3/6] Replacing text in source files..."

$extensions = @("*.cs","*.csproj","*.sln","*.resx","*.txt","*.json","*.xaml","*.config","*.props","*.targets","*.designer.cs")

$replacements = [ordered]@{
    # Namespace / using imports — most specific first
    'using PKHeX\.Core;'              = 'using Roto.Core;'
    'using PKHeX\.Core\.'             = 'using Roto.Core.'
    'using static PKHeX\.Core\.'      = 'using static Roto.Core.'
    'namespace PKHeX\.WinForms'       = 'namespace RotoDex.Desktop'
    'using PKHeX\.WinForms'           = 'using RotoDex.Desktop'
    'using static PKHeX\.WinForms\.'  = 'using static RotoDex.Desktop.'
    'namespace PKHeX\.Drawing\.PokeSprite' = 'namespace RotoDex.Drawing.PokeSprite'
    'using PKHeX\.Drawing\.PokeSprite'= 'using RotoDex.Drawing.PokeSprite'
    'namespace PKHeX\.Drawing\.Misc'  = 'namespace RotoDex.Drawing.Misc'
    'using PKHeX\.Drawing\.Misc'      = 'using RotoDex.Drawing.Misc'
    'namespace PKHeX\.Drawing'        = 'namespace RotoDex.Drawing'
    'using PKHeX\.Drawing'            = 'using RotoDex.Drawing'
    'namespace PKHeX\.Core'           = 'namespace Roto.Core'
    # Project file references
    'PKHeX\.Drawing\.PokeSprite\\\\PKHeX\.Drawing\.PokeSprite\.csproj' = 'PKHeX.Drawing.PokeSprite\RotoDex.Drawing.PokeSprite.csproj'
    'PKHeX\.Drawing\.Misc\\\\PKHeX\.Drawing\.Misc\.csproj'            = 'PKHeX.Drawing.Misc\RotoDex.Drawing.Misc.csproj'
    'PKHeX\.Drawing\\\\PKHeX\.Drawing\.csproj'                        = 'PKHeX.Drawing\RotoDex.Drawing.csproj'
    # Class / type renames
    'PKHeXSettings'                   = 'RotoDexSettings'
    # Inline namespace references (qualified names in code)
    'PKHeX\.WinForms\.Controls\.'     = 'RotoDex.Desktop.Controls.'
    'PKHeX\.WinForms\.Properties\.'   = 'RotoDex.Desktop.Properties.'
    'PKHeX\.WinForms\.'               = 'RotoDex.Desktop.'
    'PKHeX\.Drawing\.PokeSprite\.'    = 'RotoDex.Drawing.PokeSprite.'
    'PKHeX\.Drawing\.Misc\.'          = 'RotoDex.Drawing.Misc.'
    'PKHeX\.Drawing\.'                = 'RotoDex.Drawing.'
    'PKHeX\.Core\.'                   = 'Roto.Core.'
    # String literals in resource / text files (user-visible)
    'PKHeX Backups'                   = 'RotoDex Backups'
    'Open About PKHeX'                = 'Open About RotoDex'
    'PKHeX - By Kaphotics'            = 'RotoDex'
    'PKHeX\.exe'                      = 'RotoDex.exe'
    # Fullwidth
    'ＰＫＨｅＸ'                     = 'RotoDex'
    # Generic remaining after specifics
    'PKHeX'                           = 'RotoDex'
}

$files = Get-ChildItem -Path $Root -Include $extensions -Recurse -File |
    Where-Object {
        $_.FullName -notlike "*\.git\*"  -and
        $_.FullName -notlike "*\legacy\*" -and
        $_.FullName -notlike "*\obj\*"   -and
        $_.FullName -notlike "*\bin\*"
    }

$changedCount = 0
foreach ($file in $files) {
    try {
        $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
        $original = $content
        foreach ($kv in $replacements.GetEnumerator()) {
            $content = [regex]::Replace($content, $kv.Key, $kv.Value)
        }
        if ($content -ne $original) {
            [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
            Write-Host "  Updated: $($file.FullName.Replace($Root + '\', ''))"
            $changedCount++
        }
    } catch {
        Write-Warning "  FAILED: $($file.FullName): $_"
    }
}
Write-Host "  Total files changed: $changedCount"

# ─── Step 4: Rename the PKHeX.* folders ─────────────────────────────────────
Write-Host "`n[4/6] Renaming src folders..."
$folderRenames = @(
    @{ From = "src\PKHeX.Drawing.PokeSprite"; To = "src\RotoDex.Drawing.PokeSprite" },
    @{ From = "src\PKHeX.Drawing.Misc";       To = "src\RotoDex.Drawing.Misc" },
    @{ From = "src\PKHeX.Drawing";            To = "src\RotoDex.Drawing" }
)
foreach ($r in $folderRenames) {
    $src  = Join-Path $Root $r.From
    $dest = Join-Path $Root $r.To
    if (Test-Path $src) {
        Rename-Item $src $dest -Force
        Write-Host "  Renamed folder: $($r.From) -> $($r.To)"
    } else {
        Write-Host "  SKIP (not found): $($r.From)"
    }
}

# ─── Step 5: Fix solution file paths after folder rename ────────────────────
Write-Host "`n[5/6] Fixing solution file project paths..."
$slnPath = Join-Path $Root "RotoDex.sln"
$slnContent = [System.IO.File]::ReadAllText($slnPath, [System.Text.Encoding]::UTF8)
# Update the folder paths in the .sln
$slnContent = $slnContent -replace 'src\\PKHeX\.Drawing\.PokeSprite\\RotoDex\.Drawing\.PokeSprite\.csproj', 'src\RotoDex.Drawing.PokeSprite\RotoDex.Drawing.PokeSprite.csproj'
$slnContent = $slnContent -replace 'src\\PKHeX\.Drawing\.Misc\\RotoDex\.Drawing\.Misc\.csproj',             'src\RotoDex.Drawing.Misc\RotoDex.Drawing.Misc.csproj'
$slnContent = $slnContent -replace 'src\\PKHeX\.Drawing\\RotoDex\.Drawing\.csproj',                         'src\RotoDex.Drawing\RotoDex.Drawing.csproj'
# Update display names in sln
$slnContent = $slnContent -replace '"PKHeX\.Drawing\.PokeSprite"', '"RotoDex.Drawing.PokeSprite"'
$slnContent = $slnContent -replace '"PKHeX\.Drawing\.Misc"',       '"RotoDex.Drawing.Misc"'
$slnContent = $slnContent -replace '"PKHeX\.Drawing"',             '"RotoDex.Drawing"'
[System.IO.File]::WriteAllText($slnPath, $slnContent, [System.Text.Encoding]::UTF8)
Write-Host "  Solution file updated."

# ─── Step 6: Fix csproj references to renamed drawing projects ───────────────
Write-Host "`n[6/6] Fixing project-reference paths in csproj files..."
$csprojFiles = Get-ChildItem -Path $Root -Filter "*.csproj" -Recurse -File |
    Where-Object { $_.FullName -notlike "*\legacy\*" -and $_.FullName -notlike "*\obj\*" }

foreach ($proj in $csprojFiles) {
    $c = [System.IO.File]::ReadAllText($proj.FullName, [System.Text.Encoding]::UTF8)
    $orig = $c
    $c = $c -replace '\.\.\\PKHeX\.Drawing\.PokeSprite\\RotoDex\.Drawing\.PokeSprite\.csproj', '..\RotoDex.Drawing.PokeSprite\RotoDex.Drawing.PokeSprite.csproj'
    $c = $c -replace '\.\.\\PKHeX\.Drawing\.Misc\\RotoDex\.Drawing\.Misc\.csproj',             '..\RotoDex.Drawing.Misc\RotoDex.Drawing.Misc.csproj'
    $c = $c -replace '\.\.\\PKHeX\.Drawing\\RotoDex\.Drawing\.csproj',                         '..\RotoDex.Drawing\RotoDex.Drawing.csproj'
    # Also catch any still-old paths
    $c = $c -replace '\.\.\\PKHeX\.Drawing\.PokeSprite\\PKHeX\.Drawing\.PokeSprite\.csproj', '..\RotoDex.Drawing.PokeSprite\RotoDex.Drawing.PokeSprite.csproj'
    $c = $c -replace '\.\.\\PKHeX\.Drawing\.Misc\\PKHeX\.Drawing\.Misc\.csproj',             '..\RotoDex.Drawing.Misc\RotoDex.Drawing.Misc.csproj'
    $c = $c -replace '\.\.\\PKHeX\.Drawing\\PKHeX\.Drawing\.csproj',                         '..\RotoDex.Drawing\RotoDex.Drawing.csproj'
    if ($c -ne $orig) {
        [System.IO.File]::WriteAllText($proj.FullName, $c, [System.Text.Encoding]::UTF8)
        Write-Host "  Fixed refs in: $($proj.Name)"
    }
}

Write-Host "`nDone! Run 'dotnet build src/RotoDex.Desktop' to verify."

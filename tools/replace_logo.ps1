Add-Type -AssemblyName System.Drawing
$pngPath = "C:\Users\pushk\Downloads\RotoDex.png"
$icoPath = "C:\Users\pushk\OneDrive\Desktop\RotoDex\src\RotoDex.Desktop\Resources\icon.ico"

Write-Host "Converting $pngPath to $icoPath"
$bmp = [System.Drawing.Bitmap]::FromFile($pngPath)
$icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon())
$fs = [System.IO.FileStream]::new($icoPath, [System.IO.FileMode]::Create)
$icon.Save($fs)
$fs.Close()
$bmp.Dispose()
$icon.Dispose()
Write-Host "Conversion complete."

Write-Host "Replacing about.png and main.png..."
Copy-Item $pngPath -Destination "C:\Users\pushk\OneDrive\Desktop\RotoDex\src\RotoDex.Desktop\Resources\img\Program\about.png" -Force
Copy-Item $pngPath -Destination "C:\Users\pushk\OneDrive\Desktop\RotoDex\src\RotoDex.Desktop\Resources\img\Program\main.png" -Force
Write-Host "All logos replaced."

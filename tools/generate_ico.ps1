Add-Type -AssemblyName System.Drawing
$pngPath = "C:\Users\pushk\Downloads\RotoDex.png"
$icoPath = "C:\Users\pushk\OneDrive\Desktop\RotoDex\src\RotoDex.Desktop\Resources\icon.ico"

$bmp = [System.Drawing.Bitmap]::FromFile($pngPath)

$resized = New-Object System.Drawing.Bitmap 256, 256
$g = [System.Drawing.Graphics]::FromImage($resized)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.DrawImage($bmp, 0, 0, 256, 256)

$ms = New-Object System.IO.MemoryStream
$resized.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)

$fs = New-Object System.IO.FileStream($icoPath, [System.IO.FileMode]::Create)
$bw = New-Object System.IO.BinaryWriter($fs)
$bw.Write([Int16]0)
$bw.Write([Int16]1)
$bw.Write([Int16]1)
$bw.Write([byte]0)
$bw.Write([byte]0)
$bw.Write([byte]0)
$bw.Write([byte]0)
$bw.Write([Int16]1)
$bw.Write([Int16]32)
$bw.Write([int]$ms.Length)
$bw.Write([int]22)
$bw.Write($ms.ToArray())

$bw.Flush()
$bw.Close()
$fs.Close()
$ms.Close()
$g.Dispose()
$resized.Dispose()
$bmp.Dispose()
Write-Host "Proper ICO file created!"

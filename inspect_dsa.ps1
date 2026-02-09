
try {
    $path = Resolve-Path "Libs\dsa40net.dll"
    $dsa = [System.Reflection.Assembly]::LoadFile($path)
    Write-Host "Checking Dsa class..."
    $dsaType = $dsa.GetType("ch.etel.edi.dsa.v40.Dsa")
    if ($dsaType) {
        $dsaType.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static) | ForEach-Object {
            Write-Host "Static Method: $($_.Name)"
        }
        $dsaType.GetConstructors() | ForEach-Object {
            Write-Host "Constructor: $($_.GetParameters() | ForEach-Object { $_.ParameterType.Name } | Out-String)"
        }
    }
    else {
        Write-Host "Dsa class not found."
    }

    Write-Host "Checking DsaDrive class..."
    $driveType = $dsa.GetType("ch.etel.edi.dsa.v40.DsaDrive")
    if ($driveType) {
        $driveType.GetConstructors() | ForEach-Object {
            Write-Host "Constructor: $($_.GetParameters() | ForEach-Object { $_.ParameterType.Name } | Out-String)"
        }
    }
}
catch {
    Write-Host "Error: $_"
}

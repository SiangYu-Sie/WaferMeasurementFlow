
try {
    $path = Resolve-Path "Libs\dsa40net.dll"
    $dsa = [System.Reflection.Assembly]::LoadFile($path)
    $driveType = $dsa.GetType("ch.etel.edi.dsa.v40.DsaDrive")
    if ($driveType) {
        Write-Host "DsaDrive Read/Write Methods:"
        $driveType.GetMethods() | Where-Object { $_.Name -match "Read|Write|Move" } | ForEach-Object {
            Write-Host "$($_.Name) ($($_.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name } | Out-String))"
        }
    }
}
catch {
    Write-Host "Error: $_"
}


try {
    $path = Resolve-Path "Libs\dsa40net.dll"
    $dsa = [System.Reflection.Assembly]::LoadFile($path)
    $driveType = $dsa.GetType("ch.etel.edi.dsa.v40.DsaDrive")
    if ($driveType) {
        Write-Host "DsaDrive Methods:"
        $driveType.GetMethods() | Where-Object { $_.Name -match "Open|Create" } | ForEach-Object {
            Write-Host "$($_.Name) ($($_.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name } | Out-String))"
        }
    }
    
    $gatewayType = $dsa.GetType("ch.etel.edi.dsa.v40.DsaGateway")
    if ($gatewayType) {
        Write-Host "DsaGateway Methods:"
        $gatewayType.GetMethods() | Where-Object { $_.Name -match "Open|Create" } | ForEach-Object {
            Write-Host "$($_.Name) ($($_.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name } | Out-String))"
        }
    }
}
catch {
    Write-Host "Error: $_"
}

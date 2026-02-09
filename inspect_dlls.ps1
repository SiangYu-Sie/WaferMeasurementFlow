
try {
    $path = Resolve-Path "Libs\dsa40net.dll"
    Write-Host "Loading dsa40net.dll from $path"
    $dsa = [System.Reflection.Assembly]::LoadFile($path)
    Write-Host "--- dsa40net.dll Types ---"
    $dsa.GetTypes() | Where-Object { $_.IsPublic } | ForEach-Object {
        Write-Host "Type: $($_.FullName)"
        $methods = $_.GetMethods() | Where-Object { $_.IsPublic -and -not $_.IsSpecialName }
        if ($methods.Count -gt 0) {
            Write-Host "  Methods:"
            $methods | ForEach-Object {
                Write-Host "    $($_.Name) ($($_.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name } | Out-String))"
            }
        }
    }
}
catch {
    Write-Host "Error loading dsa40net.dll: $_"
}

try {
    $path = Resolve-Path "Libs\dmd40net.dll"
    Write-Host "Loading dmd40net.dll from $path"
    $dmd = [System.Reflection.Assembly]::LoadFile($path)
    Write-Host "--- dmd40net.dll Types ---"
    $dmd.GetTypes() | Where-Object { $_.IsPublic } | ForEach-Object {
        Write-Host "Type: $($_.FullName)"
        $methods = $_.GetMethods() | Where-Object { $_.IsPublic -and -not $_.IsSpecialName }
        if ($methods.Count -gt 0) {
            Write-Host "  Methods:"
            $methods | ForEach-Object {
                Write-Host "    $($_.Name) ($($_.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name } | Out-String))"
            }
        }
    }
}
catch {
    Write-Host "Error loading dmd40net.dll: $_"
}

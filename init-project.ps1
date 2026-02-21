param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,

    [string]$SourcePath = "c:\Users\112011\專案\多個AGENT_測試\WaferMeasurementFlow",
    [string]$TargetPath = "."
)

$OldName = "WaferMeasurementFlow"
$NewName = $ProjectName

Write-Host ">>> Starting Project Templatization: $OldName -> $NewName" -ForegroundColor Cyan

# 1. Define excludes
$ExcludeList = @(".git", "bin", "obj", ".vs", "build_log.txt", "build_output.txt")

# 2. Copy files to TargetPath if TargetPath is not SourcePath
if ($TargetPath -ne "." -and (Resolve-Path $TargetPath).Path -ne (Resolve-Path $SourcePath).Path) {
    if (!(Test-Path $TargetPath)) { New-Item -ItemType Directory -Path $TargetPath }
    Get-ChildItem -Path $SourcePath -Exclude $ExcludeList | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $TargetPath -Recurse -Force
    }
}

Set-Location $TargetPath

# 3. Rename Directories first (Deepest first)
Write-Host ">>> Renaming Directories..." -ForegroundColor Yellow
Get-ChildItem -Recurse -Directory | Where-Object { $_.Name -like "*$OldName*" } | Sort-Object { $_.FullName.Length } -Descending | ForEach-Object {
    $newNameDir = $_.Name.Replace($OldName, $NewName)
    Rename-Item -Path $_.FullName -NewName $newNameDir
}

# 4. Rename Files
Write-Host ">>> Renaming Files..." -ForegroundColor Yellow
Get-ChildItem -Recurse -File | Where-Object { $_.Name -like "*$OldName*" } | ForEach-Object {
    $newNameFile = $_.Name.Replace($OldName, $NewName)
    Rename-Item -Path $_.FullName -NewName $newNameFile
}

# 5. Content Replacement (Namespaces, References, IDs)
Write-Host ">>> Replacing Content strings..." -ForegroundColor Yellow
$Extensions = @("*.cs", "*.sln", "*.csproj", "*.config", "*.resx", "*.md", "*.json")
$FilesToProcess = Get-ChildItem -Recurse -Include $Extensions | Where-Object { $_.FullName -notmatch "\\(bin|obj|\.vs)\\" }

foreach ($file in $FilesToProcess) {
    try {
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        if ($content -match $OldName) {
            $newContent = $content.Replace($OldName, $NewName)
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8
            Write-Host "Processed: $($file.Name)" -ForegroundColor Gray
        }
    } catch {
        Write-Error "Failed to process $($file.Name): $_"
    }
}

Write-Host "`n>>> Done! Project '$NewName' is ready." -ForegroundColor Green
Write-Host ">>> Suggestions:" -ForegroundColor Cyan
Write-Host "1. Open the .sln file and check if all projects load correctly."
Write-Host "2. Run a full build to verify library references."
Write-Host "3. Update User Manual or README with project-specific details."

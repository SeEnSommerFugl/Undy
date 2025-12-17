# =========================================
# Visual Studio Cache Clean Script
# Deletes: .vs | bin | obj
# Then silently: dotnet restore + Clean + Rebuild
# Only shows messages relevant to THIS script
# =========================================

Write-Host "=== Visual Studio cache clean ===" -ForegroundColor Cyan

$root = Get-Location

# --- Delete .vs ---
$vsFolder = Join-Path $root ".vs"
if (Test-Path $vsFolder) {
    Write-Host "[.vs] Deleting .vs folder..." -ForegroundColor Yellow
    Remove-Item $vsFolder -Recurse -Force -ErrorAction Stop
} else {
    Write-Host "[.vs] No .vs folder found (skipping)" -ForegroundColor DarkGray
}

# --- Delete bin folders ---
Write-Host "[bin] Deleting all bin folders..." -ForegroundColor Yellow
Get-ChildItem -Path $root -Directory -Filter "bin" -Recurse -ErrorAction SilentlyContinue |
    ForEach-Object {
        Write-Host " - Removing $($_.FullName)" -ForegroundColor DarkGray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }

# --- Delete obj folders ---
Write-Host "[obj] Deleting all obj folders..." -ForegroundColor Yellow
Get-ChildItem -Path $root -Directory -Filter "obj" -Recurse -ErrorAction SilentlyContinue |
    ForEach-Object {
        Write-Host " - Removing $($_.FullName)" -ForegroundColor DarkGray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }

# --- Find solution file ---
$sln = Get-ChildItem -Path $root -Filter *.sln | Select-Object -First 1
if (-not $sln) {
    Write-Host "[ERROR] No solution (.sln) file found in this folder." -ForegroundColor Red
    Read-Host "`nPress ENTER to close this window..."
    exit 1
}

Write-Host "[sln] Using solution: $($sln.Name)" -ForegroundColor Cyan

# --- Check dotnet CLI is available ---
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Host "[ERROR] 'dotnet' CLI was not found on PATH. Cannot restore packages." -ForegroundColor Red
    Read-Host "`nPress ENTER to close this window..."
    exit 1
}

# --- Find MSBuild ---
$msbuild = Get-ChildItem "C:\Program Files\Microsoft Visual Studio" -Recurse -Filter "msbuild.exe" -ErrorAction SilentlyContinue |
           Sort-Object FullName -Descending |
           Select-Object -First 1

if (-not $msbuild) {
    Write-Host "[ERROR] MSBuild was not found under 'C:\Program Files\Microsoft Visual Studio'." -ForegroundColor Red
    Read-Host "`nPress ENTER to close this window..."
    exit 1
}

Write-Host "[msbuild] Using: $($msbuild.FullName)" -ForegroundColor Cyan

# --- NuGet restore (silent) ---
Write-Host "[restore] Running 'dotnet restore' (output suppressed)..." -ForegroundColor Yellow
dotnet restore $sln.FullName > $null 2>&1

# --- Clean (silent) ---
Write-Host "[build] Running MSBuild Clean (output suppressed)..." -ForegroundColor Yellow
& $msbuild.FullName $sln.FullName /t:Clean /p:Configuration=Debug > $null 2>&1

# --- Rebuild (silent) ---
Write-Host "[build] Running MSBuild Rebuild (output suppressed)..." -ForegroundColor Yellow
& $msbuild.FullName $sln.FullName /t:Rebuild /p:Configuration=Debug > $null 2>&1

Write-Host "`n=== Done ===" -ForegroundColor Green
Write-Host "Cache cleaning + restore + build-steps have been executed." -ForegroundColor DarkGray
Write-Host "Any compile/XAML errors (if any) are NOT shown here." -ForegroundColor DarkGray

Read-Host "`nPress ENTER to close this window..."
exit 0

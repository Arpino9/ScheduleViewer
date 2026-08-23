[CmdletBinding()]
param(
    [string]$WebUrl = "http://localhost:5124",
    [string]$ApiUrl = "http://localhost:9080"
)

$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot
$apiDirectory = Join-Path $repoRoot "schedule-viewer-api"
$apiJar = Join-Path $apiDirectory "api\target\api-1.0.0-SNAPSHOT.jar"
$mavenRepository = Join-Path $repoRoot ".m2-repository"
$webProject = Join-Path $repoRoot "ScheduleViewer.Web\ScheduleViewer.Web.csproj"
$maven = Join-Path $apiDirectory "mvnw.cmd"
$javaCommand = Get-Command java.exe -ErrorAction SilentlyContinue
$dotnetCommand = Get-Command dotnet.exe -ErrorAction SilentlyContinue
$apiProcess = $null
$normalizedApiUrl = $ApiUrl.TrimEnd('/')

function Test-ApiReady {
    try {
        $null = Invoke-WebRequest -UseBasicParsing "$normalizedApiUrl/api/calendar/status" -TimeoutSec 2
        return $true
    }
    catch {
        return $false
    }
}

try {
    if ($null -eq $dotnetCommand) {
        throw ".NET SDK was not found on PATH. Install .NET 10 SDK."
    }

    if (-not (Test-ApiReady)) {
        if (-not (Test-Path -LiteralPath $maven)) {
            throw "Maven wrapper was not found: $maven"
        }
        if ($null -eq $javaCommand) {
            throw "Java was not found on PATH. Install Java 21 or newer."
        }

        Write-Host "Building ScheduleViewer API..." -ForegroundColor Cyan
        & $maven `
            "-Dmaven.repo.local=$mavenRepository" `
            package `
            -pl api `
            -am `
            -DskipTests `
            --no-transfer-progress
        if ($LASTEXITCODE -ne 0) {
            throw "ScheduleViewer API failed to build (exit code $LASTEXITCODE)."
        }

        Write-Host "Starting ScheduleViewer API..." -ForegroundColor Cyan
        $apiProcess = Start-Process `
            -FilePath $javaCommand.Source `
            -ArgumentList @("-jar", "`"$apiJar`"") `
            -WorkingDirectory $apiDirectory `
            -WindowStyle Hidden `
            -PassThru

        $ready = $false
        for ($attempt = 0; $attempt -lt 60; $attempt++) {
            if ($apiProcess.HasExited) {
                throw "ScheduleViewer API failed to start (exit code $($apiProcess.ExitCode))."
            }
            if (Test-ApiReady) {
                $ready = $true
                break
            }
            Start-Sleep -Seconds 1
        }

        if (-not $ready) {
            throw "ScheduleViewer API startup timed out."
        }
    }

    Write-Host "API: $normalizedApiUrl" -ForegroundColor Green
    Write-Host "Web: $WebUrl" -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop." -ForegroundColor DarkGray

    & $dotnetCommand.Source run --project $webProject --urls $WebUrl
}
finally {
    if ($null -ne $apiProcess -and -not $apiProcess.HasExited) {
        Write-Host "Stopping ScheduleViewer API..." -ForegroundColor DarkGray
        Stop-Process -Id $apiProcess.Id -Force
    }
}

[CmdletBinding()]
param(
    [string]$WebUrl = "http://localhost:5124",
    [string]$ApiUrl = "http://localhost:9080"
)

$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot
$apiDirectory = Join-Path $repoRoot "schedule-viewer-api"
$webProject = Join-Path $repoRoot "ScheduleViewer.Web\ScheduleViewer.Web.csproj"
$maven = "C:\Users\okaji\Downloads\apache-maven-3.9.14-bin\apache-maven-3.9.14\bin\mvn.cmd"
$javaHome = "C:\Program Files\Java\jdk-21"
$apiProcess = $null

function Test-ApiReady {
    try {
        $null = Invoke-WebRequest -UseBasicParsing "$ApiUrl/api/calendar/status" -TimeoutSec 2
        return $true
    }
    catch {
        return $false
    }
}

try {
    if (-not (Test-ApiReady)) {
        if (-not (Test-Path -LiteralPath $maven)) {
            throw "Maven was not found: $maven"
        }
        if (-not (Test-Path -LiteralPath $javaHome)) {
            throw "Java 21 was not found: $javaHome"
        }

        Write-Host "Starting ScheduleViewer API..." -ForegroundColor Cyan
        $env:JAVA_HOME = $javaHome
        $apiProcess = Start-Process `
            -FilePath $maven `
            -ArgumentList @("spring-boot:run", "-pl", "api", "--no-transfer-progress") `
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

    Write-Host "API: $ApiUrl" -ForegroundColor Green
    Write-Host "Web: $WebUrl" -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop." -ForegroundColor DarkGray

    dotnet run --project $webProject --urls $WebUrl
}
finally {
    if ($null -ne $apiProcess -and -not $apiProcess.HasExited) {
        Write-Host "Stopping ScheduleViewer API..." -ForegroundColor DarkGray
        taskkill.exe /PID $apiProcess.Id /T /F | Out-Null
    }
}

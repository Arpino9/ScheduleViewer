[CmdletBinding()]
param(
    [string]$WebUrl = "http://localhost:5124",
    [string]$ApiUrl = "http://localhost:9080",
    [switch]$ReuseExistingWeb
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
$normalizedWebUrl = $WebUrl.TrimEnd('/')

function Test-ApiReady {
    try {
        $null = Invoke-WebRequest -UseBasicParsing "$normalizedApiUrl/api/calendar/status" -TimeoutSec 2
        return $true
    }
    catch {
        return $false
    }
}

function Test-WebReady {
    try {
        $response = Invoke-WebRequest -UseBasicParsing $normalizedWebUrl -TimeoutSec 2
        return $response.StatusCode -ge 200 -and $response.StatusCode -lt 500
    }
    catch {
        return $false
    }
}

function Get-WebListenerProcessIds {
    $port = ([Uri]$normalizedWebUrl).Port
    $pattern = ":$port\s+.*LISTENING\s+(\d+)\s*$"

    @(netstat.exe -ano -p tcp | Select-String -Pattern $pattern | ForEach-Object {
        if ($_.Matches.Count -gt 0) {
            [int]$_.Matches[0].Groups[1].Value
        }
    } | Sort-Object -Unique)
}

function Test-IsScheduleViewerWebProcess {
    param([System.Diagnostics.Process]$Process)

    if ($Process.ProcessName -eq "ScheduleViewer.Web") {
        return $true
    }
    if ($Process.ProcessName -ne "dotnet") {
        return $false
    }

    try {
        $commandLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $($Process.Id)").CommandLine
        return $commandLine -like "*ScheduleViewer.Web*"
    }
    catch {
        return $false
    }
}

function Get-ValidatedWebListenerProcessIds {
    $listenerProcessIds = @(Get-WebListenerProcessIds)
    foreach ($listenerProcessId in $listenerProcessIds) {
        $listenerProcess = Get-Process -Id $listenerProcessId -ErrorAction SilentlyContinue
        if ($null -eq $listenerProcess) {
            continue
        }
        if (-not (Test-IsScheduleViewerWebProcess $listenerProcess)) {
            throw "Port $(([Uri]$normalizedWebUrl).Port) is being used by $($listenerProcess.ProcessName) (PID $listenerProcessId). Stop it before starting ScheduleViewer."
        }
    }
    return $listenerProcessIds
}

function Stop-ExistingWebServer {
    $listenerProcessIds = @(Get-ValidatedWebListenerProcessIds)
    foreach ($listenerProcessId in $listenerProcessIds) {
        if ($null -eq (Get-Process -Id $listenerProcessId -ErrorAction SilentlyContinue)) {
            continue
        }
        Write-Host "Stopping the existing ScheduleViewer Web process (PID $listenerProcessId)..." -ForegroundColor DarkGray
        Stop-Process -Id $listenerProcessId -Force
        Wait-Process -Id $listenerProcessId -Timeout 10 -ErrorAction SilentlyContinue
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
    Write-Host "Web: $normalizedWebUrl" -ForegroundColor Green

    if ((Test-WebReady) -and $ReuseExistingWeb) {
        $null = @(Get-ValidatedWebListenerProcessIds)
        Write-Host "Web is already running. Reusing the existing process." -ForegroundColor Yellow
        if ($null -ne $apiProcess) {
            Write-Host "Press Ctrl+C to stop the API." -ForegroundColor DarkGray
            Wait-Process -Id $apiProcess.Id
        }
        return
    }

    Stop-ExistingWebServer
    Write-Host "Press Ctrl+C to stop." -ForegroundColor DarkGray

    & $dotnetCommand.Source run --project $webProject --urls $WebUrl
}
finally {
    if ($null -ne $apiProcess -and -not $apiProcess.HasExited) {
        Write-Host "Stopping ScheduleViewer API..." -ForegroundColor DarkGray
        Stop-Process -Id $apiProcess.Id -Force
    }
}

@echo off
cd /d C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api

set MVN=C:\Users\okaji\Downloads\apache-maven-3.9.14-bin\apache-maven-3.9.14\bin\mvn.cmd
set JAVA_HOME=C:\Program Files\Java\jdk-21
set LOG=C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api\server.log

if not exist "certs\server.pem" (
    echo.
    echo  [ERROR] 証明書が見つかりません。先に setup-https.bat を管理者権限で実行してください。
    echo.
    pause
    exit /b 1
)

:: Kill existing process on ports 9080 and 9443
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":9080 "') do (
    taskkill /F /PID %%p >nul 2>&1
)
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":9443 "') do (
    taskkill /F /PID %%p >nul 2>&1
)

echo.
echo  Starting ScheduleViewer API (HTTPS/9443)...
echo  Log: %LOG%
echo  URL: https://192.168.64.68:9443/
echo  Close this window to stop the server.
echo.

echo [%DATE% %TIME%] Starting ScheduleViewer API (HTTPS)... >> "%LOG%"

start /B powershell -Command "for ($i=0; $i -lt 60; $i++) { Start-Sleep 3; try { [Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}; $r = Invoke-WebRequest 'https://localhost:9443/' -UseBasicParsing; Start-Process 'https://localhost:9443/'; break } catch {} }"

%MVN% spring-boot:run -pl api --no-transfer-progress -Dspring-boot.run.profiles=https >> "%LOG%" 2>&1
echo [%DATE% %TIME%] ScheduleViewer API stopped. >> "%LOG%"

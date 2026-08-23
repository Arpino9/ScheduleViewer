@echo off
cd /d C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api

set MVN=C:\Users\okaji\Downloads\apache-maven-3.9.14-bin\apache-maven-3.9.14\bin\mvn.cmd
set JAVA_HOME=C:\Program Files\Java\jdk-21
set LOG=C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api\server.log

:: Kill existing process on port 9080
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":9080 "') do (
    taskkill /F /PID %%p >nul 2>&1
)
:: ポートが TIME_WAIT から解放されるまで少し待つ
timeout /T 3 /NOBREAK >nul 2>&1

echo.
echo  Starting ScheduleViewer API...
echo  Log: %LOG%
echo  Close this window to stop the server.
echo.

echo [%DATE% %TIME%] Starting ScheduleViewer API... >> "%LOG%"

:: Open browser automatically when server is ready
start /B powershell -Command "for ($i=0; $i -lt 60; $i++) { Start-Sleep 3; try { $c = New-Object System.Net.Sockets.TcpClient('localhost', 9080); $c.Close(); Start-Process 'http://localhost:9080/'; break } catch {} }"

rem %MVN% spring-boot:run -pl api --no-transfer-progress >> "%LOG%" 2>&1
%MVN% spring-boot:run -pl api --no-transfer-progress >> "%LOG%" 2>&1
echo [%DATE% %TIME%] ScheduleViewer API stopped. >> "%LOG%"

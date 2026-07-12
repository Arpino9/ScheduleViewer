@echo off
cd /d C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api

set MVN=C:\Users\okaji\Downloads\apache-maven-3.9.14-bin\apache-maven-3.9.14\bin\mvn.cmd
set JAVA_HOME=C:\Program Files\Java\jdk-21
set LOG=C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api\server.log

:: ポート9080を使用している既存プロセスを終了
for /f "tokens=5" %%p in ('netstat -ano ^| findstr ":9080 "') do (
    taskkill /F /PID %%p >nul 2>&1
)

echo.
echo  ScheduleViewer API 起動中...
echo  ログ: %LOG%
echo  停止するにはこのウィンドウを閉じてください
echo.

echo [%DATE% %TIME%] ScheduleViewer API 起動中... >> "%LOG%"
%MVN% spring-boot:run -pl api --no-transfer-progress >> "%LOG%" 2>&1
echo [%DATE% %TIME%] ScheduleViewer API 終了 >> "%LOG%"

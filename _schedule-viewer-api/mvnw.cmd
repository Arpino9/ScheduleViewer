@echo off
setlocal

set "BASEDIR=%~dp0"
set "MAVEN_VERSION=3.9.9"
set "MAVEN_HOME=%USERPROFILE%\.m2\wrapper\dists\apache-maven-%MAVEN_VERSION%"
set "MVN_CMD=%MAVEN_HOME%\bin\mvn.cmd"

if not exist "%MVN_CMD%" (
    echo Downloading Apache Maven %MAVEN_VERSION%...
    set "ZIP_URL=https://repo.maven.apache.org/maven2/org/apache/maven/apache-maven/%MAVEN_VERSION%/apache-maven-%MAVEN_VERSION%-bin.zip"
    set "ZIP_FILE=%TEMP%\apache-maven-%MAVEN_VERSION%-bin.zip"
    powershell -Command "Invoke-WebRequest -Uri '%ZIP_URL%' -OutFile '%ZIP_FILE%'"
    if errorlevel 1 (
        echo ERROR: Failed to download Maven. Check your internet connection.
        exit /b 1
    )
    powershell -Command "Expand-Archive -Path '%ZIP_FILE%' -DestinationPath '%USERPROFILE%\.m2\wrapper\dists' -Force"
    del "%ZIP_FILE%"
    echo Maven %MAVEN_VERSION% installed.
)

"%MVN_CMD%" -f "%BASEDIR%pom.xml" %*
endlocal

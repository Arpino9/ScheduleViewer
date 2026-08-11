@echo off
setlocal EnableDelayedExpansion

set "BASEDIR=%~dp0"
set "MAVEN_VERSION=3.9.9"
set "MAVEN_HOME=%USERPROFILE%\.m2\wrapper\dists\apache-maven-%MAVEN_VERSION%"
set "MVN_CMD=%MAVEN_HOME%\bin\mvn.cmd"

if not exist "%MVN_CMD%" (
    echo Downloading Apache Maven %MAVEN_VERSION%...
    set "ZIP_URL=https://repo.maven.apache.org/maven2/org/apache/maven/apache-maven/%MAVEN_VERSION%/apache-maven-%MAVEN_VERSION%-bin.zip"
    set "ZIP_FILE=%TEMP%\apache-maven-%MAVEN_VERSION%-bin.zip"
    powershell -NoProfile -Command "Invoke-WebRequest -Uri '!ZIP_URL!' -OutFile '!ZIP_FILE!'"
    if errorlevel 1 (
        echo ERROR: Failed to download Maven. Check your internet connection.
        exit /b 1
    )
    powershell -NoProfile -Command "Expand-Archive -Path '!ZIP_FILE!' -DestinationPath '%USERPROFILE%\.m2\wrapper\dists' -Force"
    if errorlevel 1 (
        del "!ZIP_FILE!" 2>nul
        echo ERROR: Failed to extract Maven.
        exit /b 1
    )
    del "!ZIP_FILE!"
    echo Maven %MAVEN_VERSION% installed.
)

call "%MVN_CMD%" -f "%BASEDIR%pom.xml" %*
set "MVN_EXIT_CODE=!ERRORLEVEL!"
endlocal & exit /b %MVN_EXIT_CODE%

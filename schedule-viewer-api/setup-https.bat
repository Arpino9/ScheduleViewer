@echo off
:: ScheduleViewer HTTPS セットアップ
:: 初回のみ管理者権限で実行してください

set BASE=C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api
set CERTS=%BASE%\certs
set LAN_IP=192.168.64.68

echo.
echo  ScheduleViewer HTTPS セットアップ
echo  ================================
echo.

:: mkcert インストール確認
where mkcert >nul 2>&1
if errorlevel 1 (
    echo  [1/4] mkcert をインストールします...
    winget install FiloSottile.mkcert --silent
    if errorlevel 1 (
        echo.
        echo  [ERROR] winget でのインストールに失敗しました。
        echo  https://github.com/FiloSottile/mkcert/releases から
        echo  mkcert-v*-windows-amd64.exe をダウンロードして
        echo  mkcert.exe として PATH の通った場所に置いてください。
        pause
        exit /b 1
    )
    :: PATHを反映
    set "PATH=%PATH%;%LOCALAPPDATA%\Microsoft\WinGet\Packages\FiloSottile.mkcert_Microsoft.Winget.Source_8wekyb3d8bbwe"
) else (
    echo  [1/4] mkcert は導入済みです。
)

echo.
echo  [2/4] ローカルCAをシステムに登録します...
mkcert -install
if errorlevel 1 (
    echo  [WARN] CA登録に失敗しました。管理者権限で実行してください。
)

echo.
echo  [3/4] 証明書を生成します (LAN IP: %LAN_IP%)...
if not exist "%CERTS%" mkdir "%CERTS%"
mkcert -cert-file "%CERTS%\server.pem" -key-file "%CERTS%\server-key.pem" %LAN_IP% localhost 127.0.0.1
if errorlevel 1 (
    echo  [ERROR] 証明書の生成に失敗しました。
    pause
    exit /b 1
)
echo  証明書を生成しました: %CERTS%\server.pem

echo.
echo  [4/4] Windows ファイアウォールのルールを追加します...
netsh advfirewall firewall show rule name="ScheduleViewer HTTP 9080" >nul 2>&1
if errorlevel 1 (
    netsh advfirewall firewall add rule name="ScheduleViewer HTTP 9080" dir=in action=allow protocol=TCP localport=9080
    echo  ポート 9080 (HTTP) を開放しました。
) else (
    echo  ポート 9080 の規則は既に存在します。
)
netsh advfirewall firewall show rule name="ScheduleViewer HTTPS 9443" >nul 2>&1
if errorlevel 1 (
    netsh advfirewall firewall add rule name="ScheduleViewer HTTPS 9443" dir=in action=allow protocol=TCP localport=9443
    echo  ポート 9443 (HTTPS) を開放しました。
) else (
    echo  ポート 9443 の規則は既に存在します。
)

echo.
echo  ========================================
echo   セットアップ完了！
echo.
echo   HTTPS で起動するには:
echo     start-server-https.bat
echo.
echo   スマホからのアクセス URL:
echo     https://%LAN_IP%:9443/
echo.
echo   ※ スマホ側でも mkcert の CA を信頼する必要があります。
echo     Android: Settings > Security > Install certificate
echo     CA証明書のパス: %LOCALAPPDATA%\mkcert\rootCA.pem
echo  ========================================
echo.
pause

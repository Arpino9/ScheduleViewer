Dim WshShell
Set WshShell = CreateObject("WScript.Shell")

' 第2引数 0 = ウィンドウ非表示, 第3引数 False = 起動後に待機しない
WshShell.Run "cmd /c ""C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api\start-server.bat""", 0, False

Set WshShell = Nothing

Echo "Installing service"
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe EmsWinService.exe
Echo "Starting Service"
net start EmsWinService
pause
Echo "Stoping Service"
net stop EmsWinService

Echo "Uninstalling service"
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe EmsWinService.exe /u

pause
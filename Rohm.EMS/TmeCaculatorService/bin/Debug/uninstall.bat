Echo "Stoping Service"
net stop TmeCalculateService

Echo "Uninstalling service"
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe TmeCaculatorService.exe /u

pause
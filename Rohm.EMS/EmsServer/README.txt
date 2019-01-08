nfortunately the link in the exception text, http://go.microsoft.com/fwlink/?LinkId=70353, is broken. However, it used to lead to http://msdn.microsoft.com/en-us/library/ms733768.aspx which explains how to set the permissions.

It basically informs you to use the following command:

netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\user

You can get more help on the details using the help of netsh

For example: netsh http add ?

Gives help on the http add command.

2016-11-07

C:\Windows\system32>netsh http add urlacl url=http://+:7777/EmsService user=everyone

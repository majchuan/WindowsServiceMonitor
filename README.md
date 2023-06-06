# WindowsServiceMonitor
This is console application to create a windows service, which will check target windows service is running or not. if target windows service is not running, it will start this service automatically.\
This application does not use any third party schedule job. it only use backgroundservice functionality to build up a windows service. \
This application can be run on other platform too, only need minor changes. \
dotnet publish --output "file directory" \
sc.exe create "servicename" bingpath="exe file location" \
sc.exe start "servicename" \
sc.exe stop "servicename" \

Remove-EventLog -LogName "logname" \

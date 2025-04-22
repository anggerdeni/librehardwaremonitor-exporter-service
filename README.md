## Dependencies

```
dotnet add package Microsoft.AspNetCore.App
dotnet add package LibreHardwareMonitorLib --version 0.9.4
dotnet add package Microsoft.Extensions.Hosting.WindowsServices
```

## Publish

`dotnet publish -o C:\Users\angge\services\librehardwaremonitor-exporter -c Release -r win-x64 --self-contained`


## Create Service

Run as Admin
```
sc.exe create Librehardwaremonitor-Exporter binpath=C:\Users\angge\services\librehardwaremonitor-exporter\librehardwaremonitor-exporter-service.exe start=auto
```

Somehow I need to open up the program manually first then the service will start, otherwise Windows will complain about the service not responding in a timely manner.

## Reference

- https://medium.com/@adebanjoemmanuel01/running-a-worker-service-as-a-windows-service-c1d12a28a73c
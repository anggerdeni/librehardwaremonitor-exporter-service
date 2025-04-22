using librehardwaremonitor_exporter_service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Librehardwaremonitor-Exporter";
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

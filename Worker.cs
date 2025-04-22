using Microsoft.AspNetCore.Builder;
using LibreHardwareMonitor.Hardware;

namespace librehardwaremonitor_exporter_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private WebApplication? _webhost;
        private Computer computer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var builder = WebApplication.CreateBuilder();
            _webhost = builder.Build();
            _webhost.MapGet("/", () => "OK");
            _webhost.MapGet("/metrics", getMetrics);

            _webhost.Run("http://0.0.0.0:9100");
            _logger.LogInformation("Librehardwaremonitor-exporter service starting up");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Librehardwaremonitor-exporter service shutting down");
            _webhost?.StopAsync();
            return base.StopAsync(cancellationToken);
        }


        private string getMetrics()
        {
            _logger.LogInformation("Getting computer metrics at {time}", DateTimeOffset.Now);
            computer.Open();
            computer.Accept(new UpdateVisitor());
            var metrics = new List<string>();
            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (ISensor sensor in hardware.Sensors)
                {
                    metrics.Add(formatSensorData(sensor, hardware.Name));
                }
            }
            computer.Close();
            return string.Join("\n", metrics);

        }

        private static string formatSensorData(ISensor sensor, string hardwareName)
        {
            string metricName = "unknown";
            switch (sensor.SensorType)
            {
                case SensorType.Voltage:
                    metricName = "voltage";
                    break;
                case SensorType.Current:
                    metricName = "current";
                    break;
                case SensorType.Power:
                    metricName = "power";
                    break;
                case SensorType.Clock:
                    metricName = "clock";
                    break;
                case SensorType.Temperature:
                    metricName = "temperature";
                    break;
                case SensorType.Load:
                    metricName = "load";
                    break;
                case SensorType.Frequency:
                    metricName = "frequency";
                    break;
                case SensorType.Fan:
                    metricName = "fan";
                    break;
                case SensorType.Flow:
                    metricName = "flow";
                    break;
                case SensorType.Control:
                    metricName = "control";
                    break;
                case SensorType.Level:
                    metricName = "level";
                    break;
                case SensorType.Factor:
                    metricName = "factor";
                    break;
                case SensorType.Data:
                    metricName = "data";
                    break;
                case SensorType.SmallData:
                    metricName = "smallData";
                    break;
                case SensorType.Throughput:
                    metricName = "throughput";
                    break;
                case SensorType.TimeSpan:
                    metricName = "timeSpan";
                    break;
                case SensorType.Energy:
                    metricName = "energy";
                    break;
                case SensorType.Noise:
                    metricName = "noise";
                    break;
                case SensorType.Conductivity:
                    metricName = "conductivity";
                    break;
                case SensorType.Humidity:
                    metricName = "humidity";
                    break;
            }

            metricName = "librehardwaremonitor_" + metricName;
            string label = $"{sensor.Name.Replace(" ", "_").Replace("/", "_").Replace("#", "").Replace(")", "").Replace("(", "").ToLower()}";

            return $"{metricName}{{hardware=\"{hardwareName}\", sensor=\"{label}\"}} {sensor.Value.GetValueOrDefault()}";
        }
    }
}

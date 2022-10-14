using NanoServices.Script;
using Newtonsoft.Json.Linq;

namespace NanoServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static readonly JObject json = JObject.Parse(ReadJson.json);

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now}    [INFO]     Start Service");
            SaveLogs.Save($"{DateTime.Now}    [INFO]     Start Work");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"{DateTime.Now}    [Warning]     Stop Service");
            SaveLogs.Save($"{DateTime.Now}    [Warning]     Stop Service");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                float temperature;
                string temperatureString = ReadTemperature.Read(1);
                if (!float.TryParse(temperatureString, out temperature))
                {
                    _logger.LogInformation($"{DateTime.Now}     [Info]     {temperatureString}");
                    SaveLogs.Save($"{DateTime.Now}     [Info]     {temperatureString}");
                }
                else
                {
                    if(temperature > (float)json["AppSettings"]["Alert"]["High"] || temperature < (float)json["AppSettings"]["Alert"]["Low"])
                    {
                        _logger.LogInformation($"{DateTime.Now}     [Warning]     {temperature}");
                        SaveLogs.Save($"{DateTime.Now}     [Warning]     {temperature}");
                    }
                    else
                    {
                        _logger.LogInformation($"{DateTime.Now}     [Info]     {temperature}");
                        SaveLogs.Save($"{DateTime.Now}     [Info]     {temperature}");
                    }
                }
                await Task.Delay((int)json["AppSettings"]["IntervalChecker"], stoppingToken);
            }
        }
    }
}
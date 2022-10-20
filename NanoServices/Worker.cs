using NanoServices.Script;
using Newtonsoft.Json.Linq;

namespace NanoServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static readonly JObject json = JObject.Parse(ReadJson.json);
        private float temperature;
        private string actualDevice; //aktualnie wybrane u¿¹dzenie "Nazwa"
        private int numberActualSelectDevice = 1; //numer aktualnie wybranego urz¹dzenia, 1 to wartoœæ domyœlna dla pierwszego urz¹denia
        private int allDeviceNumber; //liczba wszystkich urz¹dzeñ

        //mailsender propery
        private bool canSend = true;
        private int mailCounter = (int)json["MailSettings"]["MailCounter"];
        private int sendedMail = 0;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now}    [INFO]     Start Service");
            SaveLogs.Save($"{DateTime.Now}    [INFO]     Start Work");
            allDeviceNumber = (int)json["DeviceSettings"]["DevicesCountConnect"];
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            string body = $"{DateTime.Now}    [Warning]     Stop Service";
            _logger.LogWarning(body);
            SaveLogs.Save(body);
            SendMail(body, (int)json["MailSettings"]["MailIntervalStatus"]);

            return base.StopAsync(cancellationToken);
        }

        #region LogsSave
        private void LogsPiorityZero()
        {
            if (temperature > (float)json["AppSettings"]["Alert"]["High"] || temperature < (float)json["AppSettings"]["Alert"]["Low"])
            {
                string body = $"{DateTime.Now}     [Warning]     {actualDevice}     {temperature}";
                _logger.LogInformation(body);
                SaveLogs.Save(body);

                if (canSend && sendedMail < mailCounter)
                    SendMail($"ALLERT przekroczenia temperatury krytycznej: \n{body}", (int)json["MailSettings"]["MailIntervalSend"]);
                else if (canSend && sendedMail >= mailCounter)
                    SendMail($"Aktualny status temperatury: \n{body}", (int)json["MailSettings"]["MailIntervalStatus"]);

            }
            else
            {
                string body = $"{DateTime.Now}     [Info]     {actualDevice}     {temperature}";
                _logger.LogInformation(body);
                SaveLogs.Save(body);

                sendedMail = 0;
            }
        }

        private void LogsPiorityOne()
        {
            if (temperature > (float)json["AppSettings"]["Alert"]["High"] || temperature < (float)json["AppSettings"]["Alert"]["Low"])
            {
                string body = $"{DateTime.Now}     [Warning]     {actualDevice}     {temperature}";
                _logger.LogInformation(body);
                SaveLogs.Save(body);

                if (canSend && sendedMail < mailCounter)
                    SendMail($"ALLERT przekroczenia temperatury krytycznej: \n{body}", (int)json["MailSettings"]["MailIntervalSend"]);
                else if (canSend && sendedMail >= mailCounter)
                    SendMail($"Aktualny status temperatury: \n{body}", (int)json["MailSettings"]["MailIntervalStatus"]);
            }
            else
                sendedMail = 0;
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string temperatureString = ReadTemperature.Read(numberActualSelectDevice);
                actualDevice = (string)json["DeviceSettings"][$"Device{numberActualSelectDevice}"]["Name"];
                if (!float.TryParse(temperatureString, out temperature))
                {
                    string body = $"{DateTime.Now}     [Warning]     {temperatureString}";
                    _logger.LogInformation(body);
                    SaveLogs.Save(body);
                    SendMail($"Sensor nie jest wstanie przeczytaæ wskazanej mu wartoœci {temperatureString} znajduj¹cej siê w tym komunikacie -> {body}", (int)json["MailSettings"]["MailIntervalStatus"]);
                }
                else
                {
                    if ((int)json["AppSettings"]["Logs"]["LogsPiority"] == 0)
                        LogsPiorityZero();
                    else if((int)json["AppSettings"]["Logs"]["LogsPiority"] == 1)
                        LogsPiorityOne();
                }

                numberActualSelectDevice++;
                if (numberActualSelectDevice > allDeviceNumber)
                    numberActualSelectDevice = 1;

                await Task.Delay((int)json["AppSettings"]["IntervalChecker"], stoppingToken);
            }
        }

        private async Task SendMail(string body, int delay)
        {
            canSend = false;
            sendedMail++;
            Mail mail = new Mail(new MailParams
            {
                Domain = (string)json["MailSettings"]["MailProperty"]["Domain"],
                HostSmtp = (string)json["MailSettings"]["MailProperty"]["Server"],
                Port = (int)json["MailSettings"]["MailProperty"]["Port"],
                EnableSsl = (bool)json["MailSettings"]["MailProperty"]["EnableSSl"],
                Username = (string)json["MailSettings"]["MailProperty"]["Username"],
                Login = (string)json["MailSettings"]["MailProperty"]["Login"],
                Password = (string)json["MailSettings"]["MailProperty"]["Password"]
            });

            await mail.Send(
                (string)json["MailSettings"]["MailProperty"]["MailSubject"],
                body,
                (string)json["MailSettings"]["MailProperty"]["To"]
                );

            await Task.Delay(delay);
            canSend = true;
        }
    }
}
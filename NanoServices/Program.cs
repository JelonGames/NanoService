using Microsoft.Extensions.Hosting;
using NanoServices;
using NanoServices.Script;
using Newtonsoft.Json.Linq;
using System.Web.Services.Description;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        });
}

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using MonitoringService.Services;

namespace MonitoringService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // BuildWebHost(args).Run();

            new ReportService().Report();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
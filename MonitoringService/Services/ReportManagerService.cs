using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringService.Models;

namespace MonitoringService.Services
{
    public class ReportManagerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public ReportManagerService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Report();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(int.Parse(_configuration["ReportDelay"]), stoppingToken);
            }
        }

        private void Report()
        {
            // a service provider is used because the current service is a singleton
            using (var scope = _serviceProvider.CreateScope())
            {
                var now = DateTime.Now;

                var aggregationService = scope.ServiceProvider.GetRequiredService<IAggregationService>();

                var agentAggregatedStates = aggregationService.Aggregate(now);

                Print(agentAggregatedStates, now);
            }
        }

        private void Print(IEnumerable<AgentAggregatedState> agentAggregatedStates, DateTime now)
        {
            var dir = _configuration["ReportsDirectory"];
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllLines($"{dir}/{now:yyyyMMddhhmmss}.txt",
                agentAggregatedStates.OrderBy(s => s.AgentId).Select(s => s.ToString()));
        }
    }
}
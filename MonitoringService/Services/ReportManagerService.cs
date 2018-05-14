using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MonitoringService.Models;

namespace MonitoringService.Services
{
    public class ReportManagerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<MonitoringSettings> _settings;

        public ReportManagerService(IServiceProvider serviceProvider, IOptions<MonitoringSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _settings = settings;
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

                await Task.Delay(_settings.Value.ReportDelay, stoppingToken);
            }
        }

        private void Report()
        {
            // a service provider is used because the current service is a singleton
            using (var scope = _serviceProvider.CreateScope())
            {
                var aggregationService = scope.ServiceProvider.GetRequiredService<IAggregationService>();

                var now = DateTime.Now;

                var agentAggregatedStates = aggregationService.Aggregate(now);

                Print(agentAggregatedStates, now);
            }
        }

        private void Print(IEnumerable<AgentAggregatedState> agentAggregatedStates, DateTime now)
        {
            if (!Directory.Exists(_settings.Value.ReportsDirectory))
            {
                Directory.CreateDirectory(_settings.Value.ReportsDirectory);
            }

            var filename = $"{_settings.Value.ReportsDirectory}/{now:yyyyMMddhhmmss}.txt";
            File.WriteAllLines(filename, agentAggregatedStates.OrderBy(s => s.AgentId).Select(s => s.ToString()));

            Console.WriteLine($"{filename} was created.");
        }
    }
}
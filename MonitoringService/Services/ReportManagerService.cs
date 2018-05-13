using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonitoringService.Models;

namespace MonitoringService.Services
{
    public class ReportManagerService : BackgroundService
    {
        private readonly IAggregationService _aggregationService;

        public ReportManagerService(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Report();

                // todo: move reports dir to config
                await Task.Delay(10000, stoppingToken);
            }
        }

        private void Report()
        {
            var now = DateTime.Now;

            var agentAggregatedStates = _aggregationService.Aggregate(now);

            Print(agentAggregatedStates, now);
        }

        private static void Print(IEnumerable<AgentAggregatedState> agentAggregatedStates, DateTime now)
        {
            // todo: move reports dir to config
            const string dir = "Reports";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllLines($"{dir}/{now:yyyyMMddhhmmss}.txt", agentAggregatedStates.Select(s => s.ToString()));
        }
    }
}
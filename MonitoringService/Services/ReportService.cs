using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MonitoringService.Models;
using MonitoringService.Repositories;

namespace MonitoringService.Services
{
    public class ReportService : BackgroundService
    {
        private readonly IAggregationService _aggregationService;

        public ReportService()
        {
            _aggregationService = new AggregationService(new UnitOfWork());
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                Thread.Sleep(5000);

                Report();
            }
        }

        public void Report()
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
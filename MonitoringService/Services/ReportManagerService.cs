using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MonitoringService.Models;

namespace MonitoringService.Services
{
    public class ReportManagerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IAggregationService _aggregationService;

        public ReportManagerService(IConfiguration configuration, IAggregationService aggregationService)
        {
            _configuration = configuration;
            _aggregationService = aggregationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Report();

                await Task.Delay(int.Parse(_configuration["ReportDelay"]), stoppingToken);
            }
        }

        private void Report()
        {
            var now = DateTime.Now;

            var agentAggregatedStates = _aggregationService.Aggregate(now);

            Print(agentAggregatedStates, now);
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
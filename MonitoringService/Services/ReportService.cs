using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            // todo: move reports dir to config
            File.WriteAllLines($"Reports/{now:yyyyMMddhhmmss}.txt", agentAggregatedStates.Select(s => s.ToString()));
        }
    }

    public class AgentAggregatedState
    {
        public int AgentId { get; set; }

        public DateTime? LastReportDate { get; set; }

        public bool IsActive { get; set; }

        public TimeSpan InactivePeriod { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(string.Empty);

            stringBuilder.AppendLine($"Agent ID: {AgentId}");
            stringBuilder.AppendLine($"Lasr Report Date: {LastReportDate}");
            stringBuilder.AppendLine($"Is Active: {IsActive}");

            if (IsActive)
            {
                stringBuilder.AppendLine("Errors:");
                if (Errors?.Any() == true)
                {
                    foreach (var error in Errors)
                    {
                        stringBuilder.AppendLine(error);
                    }
                }
            }
            else
            {
                stringBuilder.AppendLine($"Inctive Period: {InactivePeriod.TotalSeconds} seconds");
            }

            return stringBuilder.ToString();
        }
    }
}
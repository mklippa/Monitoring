using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonitoringService.Models;
using MonitoringService.Repositories;

namespace MonitoringService.Services
{
    public class ReportService : BackgroundService
    {
        private readonly AgentInfoRepository _agentInfoRepository;

        public ReportService()
        {
            _agentInfoRepository = new AgentInfoRepository();
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
            var agentInfos = _agentInfoRepository.GetAll();

            var groupedAgentInfos = agentInfos.GroupBy(x => x.AgentId)
                .Select(x => new
                {
                    x.Key,
                    Items = x.Select(i => i)
                });

            var items = new List<AgentItem>();
            var now = DateTime.Now;
            foreach (var infos in groupedAgentInfos)
            {
                var item = new AgentItem();
                item.AgentId = infos.Key;
                item.LastReportDate = infos.Items.OrderBy(x => x.ReportDate).LastOrDefault()?.ReportDate;

                var inactivePeriod = (now - infos.Items.OrderBy(x => x.CreateDate).LastOrDefault().CreateDate)
                    .TotalSeconds;

                item.IsActive = inactivePeriod <= 30;

                if (item.IsActive)
                {
                    item.Errors = infos.Items.Where(x => !x.ReportDate.HasValue)
                        .SelectMany(x => x.Errors.Select(e => e.Message)).ToArray();
                }
                else
                {
                    item.InactivePeriod = TimeSpan.FromSeconds(inactivePeriod);
                }

                items.Add(item);
            }

            // write to a file
            var stringBuilder = new StringBuilder("");
            foreach (var item in items)
            {
                stringBuilder.AppendLine($"Indentificator: {item.AgentId}");
                stringBuilder.AppendLine($"Lasr Report Date: {item.LastReportDate}");
                stringBuilder.AppendLine($"Is Active: {item.IsActive}");
                if (item.IsActive)
                {
                    stringBuilder.AppendLine("Errors:");
                    foreach (var error in item.Errors)
                    {
                        stringBuilder.AppendLine(error);
                    }
                }
                else
                {
                    stringBuilder.AppendLine($"Inctive Period: {item.InactivePeriod.TotalSeconds}");
                }
            }

            File.WriteAllText($"report.txt", stringBuilder.ToString());

            // todo: udate the report date in agent infos
            var reportedAgentInfos = new List<AgentInfo>();
            foreach (var agentInfo in agentInfos.Where(x => x.ReportDate == null))
            {
                agentInfo.ReportDate = now;
                reportedAgentInfos.Add(agentInfo);
            }

            _agentInfoRepository.Update(reportedAgentInfos);
        }
    }

    // todo: rename to AgentState
    public class AgentItem
    {
        public int AgentId { get; set; }
        public DateTime? LastReportDate { get; set; }
        public bool IsActive { get; set; }
        public string[] Errors { get; set; }
        public TimeSpan InactivePeriod { get; set; }
    }
}
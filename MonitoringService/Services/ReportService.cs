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
        private readonly IAgentAggregatedStateService _agentAggregatedStateService;

        public ReportService()
        {
            _agentAggregatedStateService = new AgentAggregatedStateService();
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

            var agentAggregatedStates = _agentAggregatedStateService.GetAgentAggregatedStates(now);

            File.WriteAllLines($"{now:s}.txt", agentAggregatedStates.Select(s => s.ToString()));
        }
    }

    public class AgentAggregatedStateService : IAgentAggregatedStateService
    {
        // todo: move to config
        // length of an active period in seconds
        private const int ActivePeriod = 30;

        public IEnumerable<AgentAggregatedState> GetAgentAggregatedStates(DateTime now)
        {
            var unaggregatedAgentStates = GetUnreportedAgentStates().ToArray();

            var groupedStates = unaggregatedAgentStates.GroupBy(x => x.AgentId)
                .Select(x => new
                {
                    AgentId = x.Key,
                    States = x.Select(i => i)
                });

            foreach (var item in groupedStates)
            {
                yield return Aggregate(item.AgentId, item.States, now);
            }

            MarkAsAggregated(unaggregatedAgentStates);
        }

        private void MarkAsAggregated(AgentState[] unaggregatedAgentStates)
        {
            throw new NotImplementedException();

            /*
             var reportedAgentInfos = new List<AgentState>();
            foreach (var agentInfo in agentInfos.Where(x => x.ReportDate == null))
            {
                agentInfo.ReportDate = now;
                reportedAgentInfos.Add(agentInfo);
            }

            _agentInfoRepository.Update(reportedAgentInfos);
             */
        }

        private AgentAggregatedState Aggregate(int agentId, IEnumerable<AgentState> agentStates, DateTime now)
        {
            var inactivePeriod = now - GetLastAgentStateSaveDate(agentId);

            var aggregatedState = new AgentAggregatedState
            {
                AgentId = agentId,
                LastReportDate = GetLastReportDate(agentId),
                IsActive = inactivePeriod.TotalSeconds <= ActivePeriod
            };


            if (aggregatedState.IsActive)
            {
                aggregatedState.Errors = GetAggregatedStateErrors(agentStates);
            }
            else
            {
                aggregatedState.InactivePeriod = inactivePeriod;
            }

            return aggregatedState;
        }

        private static IEnumerable<string> GetAggregatedStateErrors(IEnumerable<AgentState> states)
        {
            return states.SelectMany(s => s.Errors?.Select(e => e.Message) ?? Enumerable.Empty<string>());
        }

        private DateTime GetLastAgentStateSaveDate(int agentStateId)
        {
            throw new NotImplementedException();
        }

        private DateTime? GetLastReportDate(int agentStateId)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<AgentState> GetUnreportedAgentStates()
        {
            throw new NotImplementedException();
        }
    }

    public interface IAgentAggregatedStateService
    {
        IEnumerable<AgentAggregatedState> GetAgentAggregatedStates(DateTime now);
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
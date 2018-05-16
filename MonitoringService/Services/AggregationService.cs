using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MonitoringService.Models;
using MonitoringService.Models.Entities;
using MonitoringService.Repositories;

namespace MonitoringService.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IUnitOfWork _storage;
        private readonly IOptions<MonitoringSettings> _settings;

        public AggregationService(IUnitOfWork storage, IOptions<MonitoringSettings> settings)
        {
            _storage = storage;
            _settings = settings;
        }

        public IEnumerable<AgentAggregatedState> Aggregate(DateTime now)
        {
            var lastAgentStateCreateDates = _storage.AgentStateRepository.GetLastAgentStateCreateDates(now).ToArray();

            var separator = now - TimeSpan.FromMilliseconds(_settings.Value.AgentActivePeriod);

            var active = lastAgentStateCreateDates.Where(x => x.CreateDate > separator).ToArray();

            var inactive = lastAgentStateCreateDates.Except(active);

            var result = inactive.Select(item => new AgentAggregatedState
            {
                AgentId = item.AgentId,
                LastReportDate = item.CreateDate,
                IsActive = false,
                InactivePeriod = now - item.CreateDate
            }).ToList();

            var unaggregatedActiveAgentStates = Array.Empty<AgentState>();
            if (active.Any())
            {
                var activeIds = active.Select(x => x.AgentId);
                unaggregatedActiveAgentStates = _storage.AgentStateRepository.Get(
                    s => !s.ReportDate.HasValue && activeIds.Contains(s.AgentId) && s.CreateDate <= now,
                    includeProperties: MonitoringContext.ErrorsProperty).ToArray();

                result.AddRange(active.Select(item => new AgentAggregatedState
                {
                    AgentId = item.AgentId,
                    LastReportDate = item.CreateDate,
                    IsActive = true,
                    Errors = GetAggregatedStateErrors(unaggregatedActiveAgentStates.Where(s => s.AgentId == item.AgentId))
                }));
            }

            foreach (var item in result)
            {
                yield return item;
            }

            if (unaggregatedActiveAgentStates.Any())
            {
                MarkAsAggregated(unaggregatedActiveAgentStates, now);
            }
        }

        private void MarkAsAggregated(IEnumerable<AgentState> unaggregatedAgentStates, DateTime now)
        {
            foreach (var agentState in unaggregatedAgentStates)
            {
                agentState.ReportDate = now;
                _storage.AgentStateRepository.Update(agentState);
            }

            _storage.Save();
        }

        private static IEnumerable<string> GetAggregatedStateErrors(IEnumerable<AgentState> states)
        {
            return states.SelectMany(s => s.Errors?.Select(e => e.Message) ?? Enumerable.Empty<string>());
        }
    }
}
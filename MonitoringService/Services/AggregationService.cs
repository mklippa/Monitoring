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
            var unaggregatedAgentStates = GetUnreportedAgentStates().ToArray();

            var groupedStates = unaggregatedAgentStates.GroupBy(x => x.AgentId)
                .Select(x => new
                {
                    AgentId = x.Key,
                    States = x.Select(i => i)
                }).ToList();

            var restAgents = _storage.AgentStateRepository.GetAgentIds()
                .Except(groupedStates.Select(x => x.AgentId))
                .Select(x => new
                {
                    AgentId = x,
                    States = Enumerable.Empty<AgentState>()
                }).ToList();

            groupedStates.AddRange(restAgents);

            foreach (var item in groupedStates)
            {
                yield return Aggregate(item.AgentId, item.States, now);
            }

            MarkAsAggregated(unaggregatedAgentStates, now);
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

        private AgentAggregatedState Aggregate(int agentId, IEnumerable<AgentState> agentStates, DateTime now)
        {
            var inactivePeriod = now - GetLastAgentStateSaveDate(agentId);

            var aggregatedState = new AgentAggregatedState
            {
                AgentId = agentId,
                LastReportDate = GetLastReportDate(agentId),
                IsActive = inactivePeriod.TotalMilliseconds <= _settings.Value.AgentActivePeriod
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

        private DateTime GetLastAgentStateSaveDate(int agentId)
        {
            var date = _storage.AgentStateRepository.GetLastAgentStateSaveDate(agentId);

            if (!date.HasValue)
            {
                throw new ArgumentException($"There are no any saved states for the agent with id = {agentId}.");
            }

            return date.Value;
        }

        private DateTime? GetLastReportDate(int agentId)
        {
            return _storage.AgentStateRepository.GetLastReportDate(agentId);
        }

        private IEnumerable<AgentState> GetUnreportedAgentStates()
        {
            return _storage.AgentStateRepository.Get(s => !s.ReportDate.HasValue,
                includeProperties: MonitoringContext.ErrorsProperty);
        }
    }
}
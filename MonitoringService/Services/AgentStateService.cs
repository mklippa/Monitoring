using System;
using System.Collections.Generic;
using System.Linq;
using MonitoringService.Models;
using MonitoringService.Repositories;

namespace MonitoringService.Services
{
    public class AgentStateService : IAgentStateService
    {
        private readonly IAgentStateRepository _agentStateRepository;

        public AgentStateService(IAgentStateRepository agentStateRepository)
        {
            _agentStateRepository = agentStateRepository;
        }

        public void SaveState(int agentId, IEnumerable<string> errors)
        {
            var agentState = new AgentState
            {
                AgentId = agentId,
                CreateDate = DateTime.Now,
                Errors = errors?.Select(error => new Error {Message = error})
            };

            _agentStateRepository.Add(agentState);
        }
    }
}
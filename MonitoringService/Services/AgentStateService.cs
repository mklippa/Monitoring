using System;
using System.Collections.Generic;
using System.Linq;
using MonitoringService.Models;
using MonitoringService.Repositories;

namespace MonitoringService.Services
{
    // todo: disposable?
    public class AgentStateService : IAgentStateService
    {
        private readonly IUnitOfWork _storage;

        public AgentStateService(IUnitOfWork storage)
        {
            _storage = storage;
        }

        public void SaveState(int agentId, IEnumerable<string> errors)
        {
            var agentState = new AgentState
            {
                AgentId = agentId,
                CreateDate = DateTime.Now,
                Errors = errors?.Select(error => new Error {Message = error}).ToArray()
            };

            _storage.AgentStateRepository.Insert(agentState);
            _storage.Save();
        }
    }
}
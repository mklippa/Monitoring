using System;
using System.Collections.Generic;
using System.Linq;
using MonitoringService.Models.Entities;

namespace MonitoringService.Repositories
{
    public class AgentStateRepository : GenericRepository<AgentState>
    {
        private readonly MonitoringContext _context;

        public AgentStateRepository(MonitoringContext context) : base(context)
        {
            _context = context;
        }

        public DateTime? GetLastAgentStateSaveDate(int agentId)
        {
            return _context.AgentStates.Where(s => s.AgentId == agentId).OrderBy(s => s.CreateDate).LastOrDefault()
                ?.CreateDate;
        }

        public IEnumerable<int> GetAgentIds()
        {
            return _context.AgentStates.Select(s => s.AgentId).Distinct().ToList();
        }
    }
}
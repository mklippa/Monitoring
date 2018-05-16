using System;
using System.Collections.Generic;
using System.Linq;
using MonitoringService.Models;
using MonitoringService.Models.Entities;

namespace MonitoringService.Repositories
{
    public class AgentStateRepository : GenericRepository<AgentState>, IAgentStateRepository
    {
        private readonly MonitoringContext _context;

        public AgentStateRepository(MonitoringContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<AgentStateCreateDate> GetLastAgentStateCreateDates(DateTime now)
        {
            return _context.AgentStates.Where(s => s.CreateDate <= now)
                .GroupBy(s => s.AgentId)
                .Select(s => new AgentStateCreateDate
                {
                    AgentId = s.Key,
                    CreateDate = s.Select(x => x.CreateDate).Max()
                }).ToList();
        }
    }
}
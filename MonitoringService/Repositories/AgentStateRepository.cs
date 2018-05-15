using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MonitoringService.Models;
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

        public IEnumerable<AgentStateCreateDate> GetLastAgentStateCreateDates()
        {
            return _context.AgentStates
                .GroupBy(s => s.AgentId)
                .Select(s => new AgentStateCreateDate
                {
                    AgentId = s.Key,
                    CreateDate = s.Select(x => x.CreateDate).Max()
                }).ToList();
        }
    }
}
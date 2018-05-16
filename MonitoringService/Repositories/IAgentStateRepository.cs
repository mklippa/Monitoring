using System;
using System.Collections.Generic;
using MonitoringService.Models;
using MonitoringService.Models.Entities;

namespace MonitoringService.Repositories
{
    public interface IAgentStateRepository : IGenericRepository<AgentState>
    {
        IEnumerable<AgentStateCreateDate> GetLastAgentStateCreateDates(DateTime now);
    }
}
using System;
using System.Collections.Generic;
using MonitoringService.Models;

namespace MonitoringService.Services
{
    public interface IAggregationService
    {
        IEnumerable<AgentAggregatedState> Aggregate(DateTime now);
    }
}
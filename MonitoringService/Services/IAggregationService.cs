using System;
using System.Collections.Generic;

namespace MonitoringService.Services
{
    public interface IAggregationService
    {
        IEnumerable<AgentAggregatedState> Aggregate(DateTime now);
    }
}
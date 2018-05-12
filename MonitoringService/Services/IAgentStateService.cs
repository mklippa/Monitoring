using System.Collections.Generic;

namespace MonitoringService.Services
{
    public interface IAgentStateService
    {
        void SaveState(int agentId, IEnumerable<string> errors);
    }
}
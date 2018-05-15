using System;

namespace MonitoringService.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAgentStateRepository AgentStateRepository { get; }
        void Save();
    }
}
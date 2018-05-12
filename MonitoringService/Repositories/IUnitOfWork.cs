using System;
using MonitoringService.Models;

namespace MonitoringService.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        AgentStateRepository AgentStateRepository { get; }
        void Save();
    }
}
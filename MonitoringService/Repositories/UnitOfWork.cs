using System;

namespace MonitoringService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private static readonly object Locker = new object();
        private readonly MonitoringContext _context;
        private IAgentStateRepository _agentStateRepository;

        public UnitOfWork(MonitoringContext context)
        {
            _context = context;
        }

        public IAgentStateRepository AgentStateRepository =>
            _agentStateRepository ?? (_agentStateRepository = new AgentStateRepository(_context));

        public void Save()
        {
            lock (Locker)
            {
                _context.SaveChanges();
            }
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
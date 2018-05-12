using Microsoft.EntityFrameworkCore;
using MonitoringService.Models;

namespace MonitoringService
{
    public class MonitoringContext : DbContext
    {
        public DbSet<AgentState> AgentStates { get; set; }
        public DbSet<Error> Errors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // todo: move connection string to config
            optionsBuilder.UseSqlite("Data Source=monitoring.db");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
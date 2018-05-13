using Microsoft.EntityFrameworkCore;
using MonitoringService.Models;
using MonitoringService.Models.Entities;

namespace MonitoringService
{
    public class MonitoringContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<AgentState> AgentStates { get; set; }
        public DbSet<Error> Errors { get; set; }

        public MonitoringContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlite(_connectionString);

            optionsBuilder.UseSqlServer(_connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        public static string ErrorsProperty => nameof(Errors);
    }
}
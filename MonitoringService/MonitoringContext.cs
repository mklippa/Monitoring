using Microsoft.EntityFrameworkCore;
using MonitoringService.Models.Entities;

namespace MonitoringService
{
    public class MonitoringContext : DbContext
    {
        public DbSet<AgentState> AgentStates { get; set; }
        public DbSet<Error> Errors { get; set; }

        public MonitoringContext(DbContextOptions<MonitoringContext> options) : base(options)
        {
        }

        public static string ErrorsProperty => nameof(Errors);
    }
}
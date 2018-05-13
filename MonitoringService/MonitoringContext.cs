using Microsoft.EntityFrameworkCore;
using MonitoringService.Models;
using MonitoringService.Models.Entities;

namespace MonitoringService
{
    public class MonitoringContext : DbContext
    {
        public DbSet<AgentState> AgentStates { get; set; }
        public DbSet<Error> Errors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // todo: move connection string to config
            // optionsBuilder.UseSqlite("Data Source=monitoring.db");

            optionsBuilder.UseSqlServer(
                "Data Source=WS-NSK-02;Initial Catalog=monitoring;Persist Security Info=True;User ID=policyone;Password=policyone");

            base.OnConfiguring(optionsBuilder);
        }

        public static string ErrorsProperty => nameof(Errors);
    }
}
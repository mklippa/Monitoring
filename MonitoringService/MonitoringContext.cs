using Microsoft.EntityFrameworkCore;
using MonitoringService.Models;

namespace MonitoringService
{
    public class MonitoringContext : DbContext
    {
        public DbSet<AgentInfo> AgentInfos { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=monitoring.db");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
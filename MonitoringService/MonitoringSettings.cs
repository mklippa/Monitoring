namespace MonitoringService
{
    public class MonitoringSettings
    {
        public int ReportDelay { get; set; }
        public string ReportsDirectory { get; set; }
        public string ConnectionString { get; set; }
        public int AgentActivePeriod { get; set; }
    }
}

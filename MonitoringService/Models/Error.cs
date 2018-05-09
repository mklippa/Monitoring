namespace MonitoringService.Models
{
    public class Error
    {
        public int ErrorId { get; set; }

        public string Message { get; set; }

        public int AgentInfoId { get; set; }

        public AgentInfo AgentInfo { get; set; }
    }
}
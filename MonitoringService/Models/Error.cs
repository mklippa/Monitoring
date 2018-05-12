namespace MonitoringService.Models
{
    public class Error
    {
        public int ErrorId { get; set; }

        public string Message { get; set; }

        public int AgentStateId { get; set; }

        public AgentState AgentState { get; set; }
    }
}
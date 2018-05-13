using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringService.Models.Entities
{
    public class Error
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorId { get; set; }

        public string Message { get; set; }

        public int AgentStateId { get; set; }

        public AgentState AgentState { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace MonitoringService.Models
{
    public class AgentState
    {
        public int AgentStateId { get; set; }

        public int AgentId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ReportDate { get; set; }

        public ICollection<Error> Errors { get; set; }
    }
}
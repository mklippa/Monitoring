using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringService.Models.Entities
{
    public class AgentState
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgentStateId { get; set; }

        public int AgentId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ReportDate { get; set; }

        public ICollection<Error> Errors { get; set; }
    }
}
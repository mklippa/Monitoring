using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitoringService.Models
{
    public class AgentAggregatedState
    {
        public int AgentId { get; set; }

        public DateTime? LastReportDate { get; set; }

        public bool IsActive { get; set; }

        public TimeSpan InactivePeriod { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(string.Empty);

            stringBuilder.AppendLine($"Agent ID: {AgentId}");
            stringBuilder.AppendLine($"Last Report Date: {LastReportDate}");
            stringBuilder.AppendLine($"Is Active: {IsActive}");

            if (IsActive)
            {
                stringBuilder.AppendLine("Errors:");
                if (Errors?.Any() == true)
                {
                    foreach (var error in Errors)
                    {
                        stringBuilder.AppendLine(error);
                    }
                }
            }
            else
            {
                stringBuilder.AppendLine($"Inctive Period: {InactivePeriod.TotalSeconds} seconds");
            }

            return stringBuilder.ToString();
        }
    }
}
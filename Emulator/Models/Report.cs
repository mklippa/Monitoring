using System.Collections.Generic;

namespace Emulator.Models
{
    public class Report
    {
        public Report(int agentId, IEnumerable<Error> errors)
        {
            AgentId = agentId;
            Errors = errors;
        }

        public int AgentId { get; }

        public IEnumerable<Error> Errors { get; }
    }
}
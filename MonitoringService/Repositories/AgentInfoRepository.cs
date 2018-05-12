using System.Collections.Generic;
using System.Linq;
using MonitoringService.Models;

namespace MonitoringService.Repositories
{
    public class AgentInfoRepository : IAgentStateRepository
    {
        public void Add(AgentState item)
        {
            using (var db = new MonitoringContext())
            {
                db.AgentInfos.Add(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<AgentState> GetAll()
        {
            using (var db = new MonitoringContext())
            {
                return db.AgentInfos.ToArray();
            }
        }

        public void Update(IEnumerable<AgentState> reportedAgentInfos)
        {
            using (var db = new MonitoringContext())
            {
                foreach (var agentInfo in reportedAgentInfos)
                {
                    var item = db.AgentInfos.SingleOrDefault(x => x.AgentId == agentInfo.AgentId);
                    if (item != null)
                    {
                        item.ReportDate = agentInfo.ReportDate;
                    }
                }

                db.SaveChanges();
            }
        }
    }

    public interface IAgentStateRepository
    {
        void Add(AgentState item);
    }
}
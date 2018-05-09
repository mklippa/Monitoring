using MonitoringService.Models;

namespace MonitoringService.Repositories
{
    public class AgentInfoRepository
    {
        public void Add(AgentInfo item)
        {
            using (var db = new MonitoringContext())
            {
                db.AgentInfos.Add(item);
                db.SaveChanges();
            }
        }
    }
}
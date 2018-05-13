using System.Threading.Tasks;
using Emulator.Services;

namespace Emulator.Models
{
    public class Agent
    {
        // todo: move to config manager
        private int DelayPeriod => Id * 5000;

        private readonly IReportService _reportService;

        public Agent(int id, IReportService reportService)
        {
            Id = id;
            _reportService = reportService;
        }

        public int Id { get; }

        public async Task RunAsync()
        {
            while (true)
            {
                SendReport();
                await Task.Delay(DelayPeriod);
            }
        }

        private void SendReport()
        {
            var report = _reportService.Generate(this);

            _reportService.SendReportAsync(report);
        }
    }
}
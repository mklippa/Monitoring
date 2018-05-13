using System.Threading.Tasks;
using Emulator.Services;

namespace Emulator.Models
{
    public class Agent
    {
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
                await Task.Delay(EmulatorSettings.Instance.DispatchPeriod * Id);
            }
        }

        private void SendReport()
        {
            var report = _reportService.Generate(this);

            _reportService.SendReportAsync(report);
        }
    }
}
using System.Threading;
using Emulator.Services;

namespace Emulator.Models
{
    public class Agent
    {
        private int Delay => Id * 5000;

        private readonly IReportService _reportService;

        public Agent(int id, IReportService reportService)
        {
            Id = id;
            _reportService = reportService;
        }

        public int Id { get; }

        public void Run()
        {
            while (true)
            {
                SendReport();
                Thread.Sleep(Delay);
            }
        }

        private void SendReport()
        {
            var report = _reportService.Generate(this);

            _reportService.SendReportAsync(report);
        }
    }
}
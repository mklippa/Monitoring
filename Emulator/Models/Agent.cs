using System;
using System.Threading;
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
            var delay = EmulatorSettings.Instance.DispatchPeriod * Id;

            while (true)
            {
                var now = DateTime.Now;

                var result = await SendReportAsync();

                Console.WriteLine($"{now:G}: The state of the agent {Id} was sent: {result}");

                await Task.Delay(delay);
            }
        }

        private async Task<bool> SendReportAsync()
        {
            var report = _reportService.Generate(this);

            return await _reportService.SendReportAsync(report);
        }
    }
}
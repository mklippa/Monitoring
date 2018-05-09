using System;
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
                SentReport();
                Thread.Sleep(Delay);
            }
        }

        private void SentReport()
        {
            var report = _reportService.Generate(this);

            throw new NotImplementedException();
        }
    }
}
using System;
using Emulator.Models;

namespace Emulator.Services
{
    public class ReportService : IReportService
    {
        private const int ErrorsLimit = 5;

        private readonly Random _random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);

        public Report Generate(Agent agent)
        {
            var count = _random.Next(ErrorsLimit + 1);

            var errors = new Error[count];

            for (var i = 0; i < errors.Length; i++)
            {
                errors[i] = new Error($"An error {i} occurred in the agent {agent.Id}.");
            }

            return new Report(errors);
        }
    }
}
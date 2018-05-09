using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Enter emulator number
            var count = 0;
            while (true)
            {
                Console.WriteLine("Please, enter the correct emulator number:");
                if (int.TryParse(Console.ReadLine(), out count))
                {
                    break;
                }
            }

            // Prepare agents
            var agents = new Agent[count];
            var reportService = new ReportService();

            for (var i = 0; i < agents.Length; i++)
            {
                agents[i] = new Agent(i + 1, reportService);
            }

            // Run emulation
            foreach (var agent in agents)
            {
                Task.Factory.StartNew(agent.Run);
            }

            Console.ReadLine();
        }
    }

    public class Agent
    {
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
                //Thread.Sleep(Id * 5000);
                Thread.Sleep(1000);
            }
        }

        private void SentReport()
        {
            var report = _reportService.Generate(this);

            Console.WriteLine($"Agent {Id} send report.");

            foreach (var error in report.Errors)
            {
                Console.WriteLine(error.Message);
            }
        }
    }

    public interface IReportService
    {
        Report Generate(Agent agent);
    }

    public class ReportService : IReportService
    {
        private const int MaxErrorCount = 5;

        private readonly Random _random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);

        public Report Generate(Agent agent)
        {
            var count = _random.Next(MaxErrorCount + 1);

            var errors = new Error[count];

            for (var i = 0; i < errors.Length; i++)
            {
                errors[i] = new Error($"Error {i} occurred in agent {agent.Id}.");
            }

            return new Report(errors);
        }
    }

    public class Report
    {
        public Report(IEnumerable<Error> errors)
        {
            Errors = errors;
        }

        public IEnumerable<Error> Errors { get; }
    }

    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
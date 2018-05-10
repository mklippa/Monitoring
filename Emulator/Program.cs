using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emulator.Models;
using Emulator.Services;

namespace Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var count = GetEmulatorsCount();

            var agents = PrepareAgents(count);

            Console.WriteLine("Please press Enter to continue, press Enter again to exit.");

            Console.ReadLine();

            RunAgents(agents);

            Console.ReadLine();
        }

        private static void RunAgents(IEnumerable<Agent> agents)
        {
            foreach (var agent in agents)
            {
                // todo: use a timer instead
                Task.Factory.StartNew(agent.Run).ContinueWith(t => ShowError(t, agent.Id));
            }
        }

        private static void ShowError(Task task, int agentId)
        {
            if (task.Exception != null)
            {
                Console.WriteLine($"Agent {agentId} finished its work with an error.");
            }
        }

        private static IEnumerable<Agent> PrepareAgents(int count)
        {
            var agents = new Agent[count];

            for (var i = 0; i < agents.Length; i++)
            {
                var reportService = new ReportService();
                agents[i] = new Agent(i + 1, reportService);
            }

            return agents;
        }

        private static int GetEmulatorsCount()
        {
            while (true)
            {
                Console.WriteLine("Please enter a valid amount of emulators:");
                if (int.TryParse(Console.ReadLine(), out var count) && count > 0)
                {
                    return count;
                }
            }
        }
    }
}
using System;
using System.Threading;

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

            for (var i = 0; i < agents.Length; i++)
            {
                agents[i] = new Agent(i + 1);
            }

            // Run emulation
            while (true)
            {
                for (var i = 0; i < agents.Length; i++)
                {
                    agents[i].SentReport();
                    Thread.Sleep(1000);
                }
            }
        }
    }

    public class Agent
    {
        public Agent(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public void SentReport()
        {
            Console.WriteLine($"Agent {Id} send report.");
        }
    }
}
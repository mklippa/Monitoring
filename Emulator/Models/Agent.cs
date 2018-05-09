using System;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000");
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(report.Errors.Select(x => x.Message));
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = client.PostAsync($"/api/monitoring/agents/{Id}/errors", content).Result;
                //string resultContent = result.Content.ReadAsStringAsync();
                //Console.WriteLine(resultContent);
            }
        }
    }
}
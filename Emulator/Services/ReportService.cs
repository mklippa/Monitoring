using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Emulator.Models;
using Microsoft.Extensions.Configuration;

namespace Emulator.Services
{
    public class ReportService : IReportService
    {
        private readonly Random _random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);

        private readonly int _errorsLimit;

        private readonly string _apiBaseUrl;

        public ReportService()
        {
            // todo: write a configuration manager
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            _apiBaseUrl = config["ApiBaseUrl"];
            _errorsLimit = int.Parse(config["ErrorsLimit"]);
        }

        public Report Generate(Agent agent)
        {
            var count = _random.Next(_errorsLimit + 1);

            var errors = new Error[count];

            for (var i = 0; i < errors.Length; i++)
            {
                errors[i] = new Error($"An error {i} occurred in the agent {agent.Id}.");
            }

            return new Report(agent.Id, errors);
        }

        public async Task<bool> SendReportAsync(Report report)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(report.Errors.Select(x => x.Message));
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync($"/api/monitoring/agents/{report.AgentId}/states", content);
                return result.IsSuccessStatusCode;
            }
        }
    }
}
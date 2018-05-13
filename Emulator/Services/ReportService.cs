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

        public Report Generate(Agent agent)
        {
            var count = _random.Next(EmulatorSettings.Instance.ErrorsLimit + 1);

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
                client.BaseAddress = new Uri(EmulatorSettings.Instance.ApiBaseUrl);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(report.Errors.Select(x => x.Message));
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var requestUri = string.Format(EmulatorSettings.Instance.RequestUrl, report.AgentId);
                try
                {
                    var result = await client.PostAsync(requestUri, content);
                    return result.IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine($"{DateTime.Now:G}: Server is not available.");
                    return false;
                }
            }
        }
    }
}
using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MonitoringService.Models;
using MonitoringService.Repositories;

namespace MonitoringService.Controllers
{
    [Route("api/[controller]")]
    public class MonitoringController : Controller
    {
        private readonly AgentInfoRepository _agentInfoRepository;

        public MonitoringController()
        {
            _agentInfoRepository = new AgentInfoRepository();
        }

        [HttpPost]
        [Route("agents/{id}/errors")]
        public IActionResult SaveErrors(string id, [FromBody] string[] errors)
        {
            try
            {
                var agentInfo = new AgentInfo
                {
                    AgentId = int.Parse(id),
                    CreateDate = DateTime.Now,
                    Errors = errors.Select(x => new Error
                    {
                        Message = x
                    }).ToList()
                };

                _agentInfoRepository.Add(agentInfo);

                return StatusCode((int) HttpStatusCode.OK);
            }
            catch
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
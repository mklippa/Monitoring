using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MonitoringService.Models;

namespace MonitoringService.Controllers
{
    [Route("api/[controller]")]
    public class MonitoringController : Controller
    {
        private readonly AgentInfoRepository _agentInfoRepository;
        private readonly ErrorRepository _errorRepository;

        public MonitoringController()
        {
            _agentInfoRepository = new AgentInfoRepository();
            _errorRepository = new ErrorRepository();
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
                    CreateDate = DateTime.Now
                };

                var agentInfoId = _agentInfoRepository.Add(agentInfo);

                var errorArray = errors.Select(x => new Error
                {
                    AgentInfoId = agentInfoId,
                    Message = x
                });

                _errorRepository.Add(errorArray);

                return StatusCode((int) HttpStatusCode.OK);
            }
            catch
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            };
        }
    }

    public class ErrorRepository
    {
        public void Add(IEnumerable<Error> errors)
        {

        }
    }

    public class AgentInfoRepository
    {
        public int Add(AgentInfo item)
        {
            return 0;
        }
    }

    public class AgentItem
    {
        public string AgentId { get; set; }
        public DateTime LastReportDate { get; set; }
        public bool IsActive { get; set; }
        public string[] Errors { get; set; }
        public TimeSpan InactivePeriod { get; set; }
    }
}
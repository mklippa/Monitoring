using System.Net;
using Microsoft.AspNetCore.Mvc;
using MonitoringService.Repositories;
using MonitoringService.Services;

namespace MonitoringService.Controllers
{
    [Route("api/[controller]")]
    public class MonitoringController : Controller
    {
        private readonly IAgentStateService _agentStateService;

        public MonitoringController(IAgentStateService agentStateService)
        {
            _agentStateService = agentStateService;
        }

        [HttpPost]
        [Route("agents/{id}/states")]
        public IActionResult SaveState(string id, [FromBody] string[] errors)
        {
            try
            {
                if (!int.TryParse(id, out var agentId) && agentId < 1)
                {
                    return StatusCode((int) HttpStatusCode.BadRequest);
                }

                _agentStateService.SaveState(agentId, errors);

                return StatusCode((int) HttpStatusCode.OK);
            }
            catch
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringService.Controllers
{
    [Route("api/[controller]")]
    public class MonitoringController : Controller
    {
        [HttpPost]
        [Route("agents/{id}/errors")]
        public IActionResult SaveErrors(string id, [FromBody] string[] errors)
        {
            try
            {
                return StatusCode((int) HttpStatusCode.OK);
            }
            catch
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            };
        }
    }
}
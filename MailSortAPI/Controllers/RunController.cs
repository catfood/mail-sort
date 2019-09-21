using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailSortAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        [HttpPost("run")]
        public ActionResult Run()
        {
            return Ok();
        }

        [HttpGet("status")]
        public ActionResult Status()
        {
            return Ok();
        }

        [HttpDelete("/item")]
        public ActionResult Stop(Guid item)
        {
            return Ok();
        }
    }
}
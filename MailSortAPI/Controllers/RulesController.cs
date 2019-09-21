using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailDTO;

namespace MailSortAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        // GET api/rules
        [HttpGet]
        public ActionResult<RulesetDTO> Get()
        {
            return new RulesetDTO { };
        }

        // PUT api/rules
        [HttpPut]
        public void Put([FromBody] RulesetDTO rules)
        {
        }

        // DELETE api/rules
        [HttpDelete]
        public void Delete()
        {
        }
    }
}

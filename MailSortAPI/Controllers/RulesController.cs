using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailDTO;
using MailSortBL;

namespace MailSortAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        const string dbFileName = @"rules.db";
        IRulesData rulesRepository;

        public RulesController(IRulesData data)
        {
            rulesRepository = data;
        }

        // GET api/rules
        [HttpGet]
        public RulesetDTO Get()
        {
            return rulesRepository.Load(dbFileName);
        }

        // PUT api/rules
        [HttpPut]
        public void Put([FromBody] RulesetDTO rules)
        {
            rulesRepository.Save(dbFileName, rules);
        }

        // DELETE api/rules
        [HttpDelete]
        public void Delete()
        {
            rulesRepository.Delete(dbFileName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Rules
{
    public class MailRules : IRulesService
    {
        public IEnumerable<Models.MailAction> GetActionsForMessage(Models.MailModel input)
        {
            throw new NotImplementedException();
        }
    }
}

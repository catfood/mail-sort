using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Rules
{
    public class MailRules : IRulesService
    {
        public IEnumerable<Models.IMailAction> GetActionsForMessage(Models.MailModel input)
        {
            return new List<Models.IMailAction>()
            {
                new Models.NullMailAction()
            };
        }
    }
}

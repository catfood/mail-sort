using System.Collections.Generic;
using MailSort.Models;

namespace MailSort.Rules
{
    public interface IRulesService
    {
        IEnumerable<MailAction> GetActionsForMessage(MailModel input);
    }
}
using System.Collections.Generic;
using MailSort.Models;

namespace MailSort.Rules
{
    public interface IRulesService
    {
        IEnumerable<IMailAction> GetActionsForMessage(MailModel input);
    }
}
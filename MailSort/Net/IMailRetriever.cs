
using System.Collections.Generic;

namespace MailSort.Net
{
    interface IMailRetriever
    {
        string IMAPHost { get; set; }
        string IMAPPassword { get; set; }
        string IMAPUserName { get; set; }

        IEnumerable<Models.MailModel> GetInbox();
        void Execute(Models.MailModel m, Models.IMailAction a);
    }
}
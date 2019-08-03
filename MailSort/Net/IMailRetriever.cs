
using System.Collections.Generic;

namespace MailSort.Net
{
    interface IMailRetriever
    {
        string IMAPHost { get; set; }
        string IMAPPassword { get; set; }
        string IMAPUserName { get; set; }

        string Hello();
        void ListMessages();
        IEnumerable<Models.MailModel> GetInbox();
        void Execute(Models.MailModel m, Models.MailAction a);
    }
}
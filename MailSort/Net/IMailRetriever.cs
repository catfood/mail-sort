
using System.Collections.Generic;

namespace MailSort.Net
{
    interface IMailRetriever
    {
        string IMAPHost { get; set; }
        string IMAPPassword { get; set; }
        string IMAPUserName { get; set; }

        void Open();
        void Close();
        IEnumerable<Models.MailModel> GetInbox();
        Models.MailActionResult Execute(Models.MailAction action, Models.MailModel input);
    }
}
using System.Collections.Generic;
using MailKit.Net.Imap;
using System.Linq;
using MailSort.Models;

namespace MailSort.Net
{
    class MailRetriever : IMailRetriever
    {
        public MailRetriever(Configuration.IConfiguration conf)
        {
            _confModel = conf.IMAP();
        }

        private IMAPConfig _confModel;

        public string IMAPHost { get; set; }
        public string IMAPUserName { get; set; }
        public string IMAPPassword { get; set; }

        public IEnumerable<MailModel> GetInbox()
        {
            string uri = $"imap:{IMAPHost}";
            using (var client = new ImapClient())
            {
                // For demo-purposes, accept all SSL certificates
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.AuthenticationMechanisms.Remove("NTLM");
                client.Connect(_confModel.IMAPHost, _confModel.IMAPPort, true);
                client.Authenticate(_confModel.IMAPUser, _confModel.IMAPPassword);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadOnly);

                foreach (var msg in inbox)
                {
                    var newModel = new MailModel
                    {
                        From = (msg.From.First() as MimeKit.MailboxAddress).Address,
                        To = msg.To.Where(t => t is MimeKit.MailboxAddress).Select(t => (t as MimeKit.MailboxAddress).Address).ToList(),
                        Subject = msg.Subject,
                        Date = msg.Date.DateTime,
                        AllHeaders = msg.Headers.Select(a => new KeyValuePair<string, string>(a.Field, a.Value)).ToList()
                };
                    yield return newModel;
                }
                client.Disconnect(true);
            }
        }
    }
}

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
        private ImapClient _client;

        public string IMAPHost { get; set; }
        public string IMAPUserName { get; set; }
        public string IMAPPassword { get; set; }

        public void Open()
        {
            _client = new ImapClient();
            // For demo-purposes, accept all SSL certificates
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.AuthenticationMechanisms.Remove("NTLM");
            _client.Connect(_confModel.IMAPHost, _confModel.IMAPPort, true);
            _client.Authenticate(_confModel.IMAPUser, _confModel.IMAPPassword);
        }

        public void Close()
        {
            _client.Disconnect(true);
        }

        public MailActionResult Execute(MailAction action, MailModel input)
        {
            switch (action.ActionType)
            {
                case MailActionType.Null:
                    break;
                case MailActionType.Archive:
                    break;
                case MailActionType.Copy:
                    break;
                case MailActionType.Delete:
                    break;
                case MailActionType.Forward:
                    break;
                case MailActionType.Move:
                    break;
                case MailActionType.Report:
                    System.Console.WriteLine($"Got an email from {input.From} to {input.To} regarding {input.Subject}");
                    break;
            }
            return new MailActionResult { Succeeded = true };
        }



        public IEnumerable<MailModel> GetInbox()
        {
            string uri = $"imap:{IMAPHost}";
            {
                // The Inbox folder is always available on all IMAP servers...
                var inbox = _client.Inbox;
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
            }
        }
    }
}

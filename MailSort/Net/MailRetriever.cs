using System;
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
            _confModel = conf.Get();
        }

        private Config _confModel;

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

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                //var hash = new Dictionary<string, List<MimeKit.MimeMessage>>();
                //var rules = new Rules.MailRules();
                foreach (var msg in inbox)
                {
                    //QuickDisplay(msg);
                    var fromAddress = msg.From.First() as MimeKit.MailboxAddress; // exception if empty
                    string fileUnder = fromAddress.Address;
                    var newModel = new Models.MailModel
                    {
                        From = (msg.From.First() as MimeKit.MailboxAddress).Address,
                        To = msg.To.Where(t => t is MimeKit.MailboxAddress).Select(t => (t as MimeKit.MailboxAddress).Address).ToList(),
                        Subject = msg.Subject,
                        Date = msg.Date.DateTime
                    };
                    yield return newModel;
                    //rules.GetActionsForMessage(newModel);
                }
                client.Disconnect(true);
            }
        }

        [Obsolete]
        void QuickDisplay(MimeKit.MimeMessage msg)
        {
            foreach (var fromAddr in msg.From)
            {
                if (fromAddr is MimeKit.MailboxAddress)
                {
                    Console.WriteLine((fromAddr as MimeKit.MailboxAddress).Address);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("We can't handle From addresses that aren't Mailbox addresses!");
                }
            }
        }

        public void Execute(MailModel m, IMailAction a)
        {
            if (a is Models.NullMailAction)
            {
                (a as Models.NullMailAction).Null();
            }
        }
    }
}

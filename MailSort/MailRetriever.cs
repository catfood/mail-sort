using System;
using System.Collections.Generic;
using MailKit.Net.Imap;
using System.Linq;

namespace MailSort
{
    class MailRetriever : IMailRetriever
    {
        public MailRetriever(Configuration.IConfiguration conf)
        {
            _confModel = conf.Get();
        }

        private Models.Config _confModel;

        public string IMAPHost { get; set; }
        public string IMAPUserName { get; set; }
        public string IMAPPassword { get; set; }

        public string Hello()
        {
            return "Hello, World!";
        }

        public void ListMessages()
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

                int count = 0;
                var hash = new Dictionary<string, List<MimeKit.MimeMessage>>();
                var rules = new Rules.MailRules();
                foreach (var msg in inbox)
                {
                    QuickDisplay(msg);
                    var fromAddress = msg.From.First() as MimeKit.MailboxAddress; // exception if empty
                    string fileUnder = fromAddress.Address;
                    if (++count >= 100) break;
                    var newModel = new Models.MailModel
                    {
                        From = (msg.From.First() as MimeKit.MailboxAddress).Address,
                        To = msg.To.Where(t => t is MimeKit.MailboxAddress).Select(t => (t as MimeKit.MailboxAddress).Address).ToList(),
                        Subject = msg.Subject,
                        Date = msg.Date.DateTime
                    };
                    rules.Apply(newModel);
                }
                client.Disconnect(true);
            }
        }

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
    }
}

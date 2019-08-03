using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Imap;

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
                foreach (var msg in inbox)
                {
                    Console.WriteLine($"{count} --> {msg.From}");
                    if (++count >= 100) break;
                }
                client.Disconnect(true);
            }
        }
    }
}

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
            System.Console.WriteLine(input.From);
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
                    MoveMessage(input, action.Args.Select(a => a.ToString()));
                    break;
                case MailActionType.Report:
                    System.Console.WriteLine($"Got an email from {input.From} to {input.To} regarding {input.Subject}");
                    break;
            }
            return new MailActionResult { Succeeded = true };
        }

        private void showAllFolders(MailKit.IMailFolder input)
        {
            input.GetSubfolders().ToList().ForEach(sf => System.Console.WriteLine(sf.FullName));
        }

        private void MoveMessage(MailModel msg, IEnumerable<string> dest)
        {
            System.Console.WriteLine($"Moving message to folder {string.Join("/",dest)}.");
            var topNamespace = _client.PersonalNamespaces.First(ns => ns.Path == "");
            var targetFolder = _client.GetFolder(topNamespace);
            showAllFolders(targetFolder);
            foreach (string destFolderElementName in dest)
            {
                var newTargetFolder = targetFolder.GetSubfolders().FirstOrDefault(f => f.Name.ToLower() == destFolderElementName.ToLower());
                if (newTargetFolder == null)
                {
                    newTargetFolder = targetFolder.Create(destFolderElementName, true);
                }
                targetFolder = newTargetFolder ?? throw new System.NotImplementedException();
            }
            System.Console.WriteLine($"I found {targetFolder.FullName}.");
            msg.Folder.MoveTo(msg.ID, targetFolder);
        }

        public IEnumerable<MailModel> GetInbox()
        {
            string uri = $"imap:{IMAPHost}";
            {
                // The Inbox folder is always available on all IMAP servers...
                var inbox = _client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadWrite);
                var items = inbox.Fetch(0, -1, MailKit.MessageSummaryItems.UniqueId);

                foreach (var i in items)
                {
                    var msg = inbox.GetMessage(i.UniqueId);
                    var newModel = new MailModel
                    {
                        ID = i.UniqueId,
                        Folder = inbox,
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

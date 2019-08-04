
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public interface IMailAction
    {
        void Execute(MailModel m);
    }

    public class NullMailAction: IMailAction
    {
        public void Execute(MailModel m)
        {

        }
    }

    public class ReportMailAction: IMailAction
    {
        public void Execute(MailModel m)
        {
            Console.WriteLine($"Got an email from {m.From} to {m.To} regarding {m.Subject}");
        }
    }

    public class CountMailAction: IMailAction
    {
        static Dictionary<string, int> _mailByFrom;
        public void Execute(MailModel m)
        {
            _mailByFrom[m.From]++;
        }
    }
}

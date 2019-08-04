using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public class MailModel
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public List<KeyValuePair<string,string>> AllHeaders { get; set; }
    }
}

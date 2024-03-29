﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public class MailModel
    {
        public MailKit.UniqueId ID { get; set; }
        public MailKit.IMailFolder Folder { get; set; } // FIXME: doesn't belong in the model
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public List<KeyValuePair<string,string>> AllHeaders { get; set; }
    }
}

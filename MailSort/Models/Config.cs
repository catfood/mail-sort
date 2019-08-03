using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public class Config
    {
        public string IMAPHost { get; set; }
        public int IMAPPort { get; set; }
        public string IMAPMode { get; set; }
        public string IMAPUser { get; set; }
        public string IMAPPassword { get; set; }
  }
}
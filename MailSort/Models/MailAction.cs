
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public enum MailActionType { Null, Delete, Move, Copy, Forward, Archive, Report };
    public class MailAction
    {
        public MailActionType ActionType { get; set; }
        public object[] Args { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public enum MailActionType { Null, Delete, Move, Copy, Forward, Archive, Report };
    public class MailAction
    {
        public MailAction()
        {
            Args = new List<object>();
        }
        public MailActionType ActionType { get; set; }
        public List<object> Args { get; set; }
    }
}

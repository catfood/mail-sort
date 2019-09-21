using System;

namespace MailDTO
{
    public class RuleDescriptorDTO
    {
        public string RuleName { get; set; }
        public string RuleType { get; set; }
        public string RuleData { get; set; }
    }

    public class ActionDescriptorDTO
    {
        public string ActionName { get; set; }
        public string ActionType { get; set; }
        public string ActionData { get; set; }
    }

    public class RulesetDTO
    {
        public string RulesetData { get; set; }
    }
}

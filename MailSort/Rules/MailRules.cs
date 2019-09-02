using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace MailSort.Rules
{
    public class Rule
    {
        public string RuleName { get; set; }
        public string MatchingFieldName { get; set; }
        public string MatchingFieldValue { get; set; }
        public Type ActionType { get; set; }
        public bool Match(Models.MailModel input)
        {
            var field = input.AllHeaders.Find(hdr => hdr.Key == MatchingFieldName);
            if (field.Key == null) return false;
            var matchre = new Regex(MatchingFieldValue);
            return matchre.IsMatch(field.Value);
        }
    }

    public class MailRules : IRulesService
    {
        private Configuration.IConfiguration _conf;
        private List<Rule> rules;

        public MailRules(Configuration.IConfiguration conf)
        {
            _conf = conf;
            using (var db = new LiteDB.LiteDatabase(@"rules.db"))
            {
                rules = db.GetCollection<Rule>("rules").FindAll().ToList();
            }
            rules.Add(new Rule
            {
                RuleName = "Default Null Rule",
                ActionType = typeof(Models.NullMailAction),
                MatchingFieldName = "From",
                MatchingFieldValue = ".*"
            });
            rules.Add(new Rule
            {
                RuleName = "Show it",
                ActionType = typeof(Models.ReportMailAction),
                MatchingFieldName = "From",
                MatchingFieldValue = "judy"
            });
        }

        public IEnumerable<Models.IMailAction> GetActionsForMessage(Models.MailModel m)
        {
            var result = new List<Models.IMailAction>();
            foreach (var rule in rules)
            {
                if (rule.Match(m))
                {
                    var action = Activator.CreateInstance(rule.ActionType) as Models.IMailAction;
                    result.Add(action);
                }
            }
            return result;
        }
    }
}

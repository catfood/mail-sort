using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

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
            return true;
        }
    }

    public class MailRules : IRulesService
    {
        private Configuration.IConfiguration _conf;
        private List<Rule> rules;

        public MailRules(Configuration.IConfiguration conf)
        {
            _conf = conf;
            rules = new List<Rule>();
            rules.Add(new Rule
            {
                RuleName = "Default Null Rule",
                ActionType = typeof(Models.NullMailAction),
                MatchingFieldName = "From",
                MatchingFieldValue = "fred"
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

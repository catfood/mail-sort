using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace MailSort.Rules
{
    public enum RuleConjunction { Any, All, One, None };

    public interface IRule
    {
        string RuleName { get; set; }
        bool Match(Models.MailModel input);
    }

    public class StringFieldRule: IRule
    {
        public string RuleName { get; set; }
        public string MatchingFieldName { get; set; }
        public string MatchingFieldValue { get; set; }
        public bool Match(Models.MailModel input)
        {
            var field = input.AllHeaders.Find(hdr => hdr.Key == MatchingFieldName);
            if (field.Key == null) return false;
            var matchre = new Regex(MatchingFieldValue);
            return matchre.IsMatch(field.Value);
        }
    }

    public class DateFieldRule: IRule
    {
        public string RuleName { get; set; }
        public string MatchingFieldName { get; set; }
        public DateTime? MinimumDateTimeValue { get; set; }
        public DateTime? MaximumDateTimeValue { get; set; }
        public bool Default { get; set; }
        public bool Match(Models.MailModel input)
        {
            var field = input.AllHeaders.Find(hdr => hdr.Key == MatchingFieldName);
            if (field.Key == null) return false;
            var MatchingFieldValue = new DateTime();
            if (DateTime.TryParse(field.Value, out MatchingFieldValue))
            {
                return (!MinimumDateTimeValue.HasValue || MatchingFieldValue >= MinimumDateTimeValue)
                    && (!MaximumDateTimeValue.HasValue || MatchingFieldValue <= MaximumDateTimeValue);
            }
            else
            {
                return Default;
            }
        }
    }

    public class CompoundRule : IRule
    {
        public string RuleName { get; set; }
        public bool Match(Models.MailModel input)
        {
            switch (Conjunction)
            {
                case RuleConjunction.All:
                    return SubordinateRules.All(r => r.Match(input));
                case RuleConjunction.Any:
                    return SubordinateRules.Any(r => r.Match(input));
                case RuleConjunction.None:
                    return !SubordinateRules.Any(r => r.Match(input));
                case RuleConjunction.One:
                    return SubordinateRules.Count(r => r.Match(input)) == 1;
            }
            return true;
        }

        public IEnumerable<IRule> SubordinateRules { get; set; }
        public RuleConjunction Conjunction { get; set; }
    }

    public class RuleActionPair
    {
        public IRule Rule { get; set; }
        public Models.MailAction Action { get; set; }
    }

    public class MailRules : IRulesService
    {
        private Configuration.IConfiguration _conf;
        private List<RuleActionPair> rules;

        public MailRules(Configuration.IConfiguration conf)
        {
            _conf = conf;
            using (var db = new LiteDB.LiteDatabase(@"rules.db"))
            {
                rules = db.GetCollection<RuleActionPair>("rules").FindAll().ToList();
            }
            var nullAction = new Models.MailAction
            {
                ActionType = Models.MailActionType.Null,
                 Args = null
            };
            var threeDayRule = new DateFieldRule
            {
                RuleName = "More than three days old",
                Default = false,
                MatchingFieldName = "Date",
                MinimumDateTimeValue = null,
                MaximumDateTimeValue = DateTime.Now.AddDays(-3)
            };
            var HARORule = new StringFieldRule
            {
                RuleName = "From HARO",
                MatchingFieldName = "From",
                MatchingFieldValue = "HARO"
            };
            var subjectRule = new StringFieldRule
            {
                RuleName = "Contains letter \"A\"",
                MatchingFieldName = "Subject",
                MatchingFieldValue = "a"
            };
            var judyRule = new StringFieldRule
            {
                RuleName = "Messages from Judy",
                MatchingFieldName = "From",
                MatchingFieldValue = "judy"
            };
            rules.Add(new RuleActionPair
            {
                Rule = judyRule,
                Action = nullAction
            });
            rules.Add(new RuleActionPair
            {
                Rule = new CompoundRule
                {
                    RuleName = "Old emails from HARO",
                    Conjunction = RuleConjunction.All,
                    SubordinateRules = new List<IRule> { HARORule, threeDayRule }
                },
                Action = nullAction
            });
        }

        public IEnumerable<Models.MailAction> GetActionsForMessage(Models.MailModel m)
        {
            var result = new List<Models.MailAction>();
            foreach (var ruleAction in rules)
            {
                if (ruleAction.Rule.Match(m))
                {
                    result.Add(ruleAction.Action);
                }
            }
            return result;
        }
    }
}

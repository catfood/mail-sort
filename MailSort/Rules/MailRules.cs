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

    public class ToOrFromRule : IRule
    {
        public string RuleName { get; set; }
        public string MatchingFieldValue { get; set; }
        public bool Match(Models.MailModel input)
        {
            var matchre = new Regex(MatchingFieldValue);
            return matchre.IsMatch(input.From) || input.To.Any(to => matchre.IsMatch(to));
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

    public class FlagRule: IRule
    {
        public string RuleName { get; set; }
        public string FlagValue { get; set; }
        public bool Match(Models.MailModel input)
        {
            return false; // FIXME
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
        public bool StopIfRun { get; set; }
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
            var haroAction = new Models.MailAction
            {
                ActionType = Models.MailActionType.Move,
                Args = { "delete-these", "HARO-delete-these" }
            };
            var jojoAction = new Models.MailAction
            {
                ActionType = Models.MailActionType.Move,
                Args = { "friends", "jojo" }
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
            var priorityFlagRule = new FlagRule
            {
                RuleName = "Priority Flag",
                FlagValue = "P"
            };
            var jojoRule = new ToOrFromRule
            {
                RuleName = "Messages to and from Jojo",
                MatchingFieldValue = "jojo@example.com"
            };

            rules.Add(new RuleActionPair
            {
                Rule = priorityFlagRule,
                Action = nullAction,
                StopIfRun = true
            });

            // Everything to or from Jojo gets stashed in the Jojo folder.
            rules.Add(new RuleActionPair
            {
                Rule = jojoRule,
                Action = jojoAction
            });

            // If it's from HARO and it's more than three days ago, make it go away.
            rules.Add(new RuleActionPair
            {
                Rule = new CompoundRule
                {
                    RuleName = "Old emails from HARO",
                    Conjunction = RuleConjunction.All,
                    SubordinateRules = new List<IRule> { HARORule, threeDayRule }
                },
                Action = haroAction
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
                    if (ruleAction.StopIfRun) break;
                }
            }
            return result;
        }
    }
}

using System;
using MailDTO;

namespace MailSortBL
{
    public class RulesData : IRulesData
    {
        public RulesetDTO Load(string dbFile)
        {
            return new RulesetDTO();
        }

        public void Save(string dbFile, RulesetDTO data)
        {

        }

        public void Delete(string dbFile)
        {
            System.IO.File.Delete(dbFile);
        }
    }
}

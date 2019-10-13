using MailDTO;

namespace MailSortBL
{
    public interface IRulesData
    {
        void Delete(string dbFile);
        RulesetDTO Load(string dbFile);
        void Save(string dbFile, RulesetDTO data);
    }
}
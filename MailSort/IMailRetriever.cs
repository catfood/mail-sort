namespace MailSort
{
    interface IMailRetriever
    {
        string IMAPHost { get; set; }
        string IMAPPassword { get; set; }
        string IMAPUserName { get; set; }

        string Hello();
        void ListMessages();
    }
}
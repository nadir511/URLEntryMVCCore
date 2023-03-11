using MimeKit;

namespace URLEntryMVC.ViewModel.EmailServiceVM
{
    public class MessageVM
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? EmailType { get; set; }
        public MessageVM(string? userName, string? password,IEnumerable<string> to, string subject, string content,string? emailType)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
            UserName = userName;
            Password = password;
            EmailType = emailType;
        }
    }
}

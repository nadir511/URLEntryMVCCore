using URLEntryMVC.ViewModel.EmailServiceVM;

namespace URLEntryMVC.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(MessageVM message);
    }
}

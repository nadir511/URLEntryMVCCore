using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
//using System.Net;
//using System.Net.Mail;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.EmailServiceVM;

namespace URLEntryMVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfigurationVM _emailConfig;
        public EmailService(EmailConfigurationVM emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public void SendEmail(MessageVM message)
        {
            //var from = _emailConfig.From;
            //var to = message.To;
            //var subject = message.Subject;
            //var body = message.Body;

            //var username = _emailConfig.UserName; // get from Mailtrap
            //var password = _emailConfig.Password; // get from Mailtrap

            //var host = _emailConfig.SmtpServer;
            //var port = _emailConfig.Port;

            //var client = new SmtpClient(host, port)
            //{
            //    Credentials = new NetworkCredential(username, password),
            //    EnableSsl = false
            //};

            //client.Send(from, to, subject, body);
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        private MimeMessage CreateEmailMessage(MessageVM message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text ="Click on this link to setup your account "+ message.Content };
            return emailMessage;
        }
        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
        public string settingUpTheEmailTemplate(string userName,string password,string callBackURL)
        {
            #region|Setting Up the Email Template|
            string FilePath = string.Empty;
            FilePath =System.IO.Path. MapPath(@"~\\Views\\Shared\\EmailTemplates\\ActivateAccountAndResetPassword.cshtml");
            string accountForgetPassText = System.IO.File.ReadAllText(FilePath);
            accountForgetPassText = accountForgetPassText.Replace("{user}", userName);
            accountForgetPassText = accountForgetPassText.Replace("{Url}", callBackURL);

            string MasterEmailTemplateFilePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\\Views\\Shared\\EmailTemplates\\MasterEmailTemplate.cshtml");
            string MasterEmailText = System.IO.File.ReadAllText(MasterEmailTemplateFilePath);
            MasterEmailText = MasterEmailText.Replace("{body}", accountForgetPassText);
            MasterEmailText = MasterEmailText.Replace("{Year}", DateTime.Now.Year.ToString());
            string body = MasterEmailText;
            body = body.Trim();
            #endregion
            return body;
        }
    }
}

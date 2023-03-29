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
        private readonly IWebHostEnvironment _environment;

        public EmailService(EmailConfigurationVM emailConfig, IWebHostEnvironment environment)
        {
            _emailConfig = emailConfig;
            _environment = environment;
        }
        public void SendEmail(MessageVM message)
        {
            try
            {
                var emailMessage = CreateEmailMessage(message);
                Send(emailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        private MimeMessage CreateEmailMessage(MessageVM message)
        {
            try
            {
                #region|Setting up the Template|
                var messageBody = settingUpTheEmailTemplate(message.UserName, message.Password, message.Content, message.EmailType);
                #endregion
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Tap-That", _emailConfig.From));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = messageBody };
                return emailMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
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
        public string settingUpTheEmailTemplate(string userName,string password,string callBackURL,string? emialType)
        {
            #region|Setting Up the Email Template|
            string accountForgetPassText = string.Empty;
            if (emialType== "forgetPassword")
            {
                var FilePath = _environment.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "ForgetPassword.cshtml";
                accountForgetPassText = System.IO.File.ReadAllText(FilePath);
                accountForgetPassText = accountForgetPassText.Replace("{user}", userName);
                accountForgetPassText = accountForgetPassText.Replace("{Url}", callBackURL);
            }
            else
            {
                var FilePath = _environment.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "ActivateAccountAndResetPassword.cshtml";
                accountForgetPassText = System.IO.File.ReadAllText(FilePath);
                accountForgetPassText = accountForgetPassText.Replace("{user}", userName);
                accountForgetPassText = accountForgetPassText.Replace("{password}", password);
                accountForgetPassText = accountForgetPassText.Replace("{Url}", callBackURL);

            }
            var MasterEmailTemplateFilePath = _environment.WebRootPath
                                              + Path.DirectorySeparatorChar.ToString()
                                              + "EmailTemplates"
                                              + Path.DirectorySeparatorChar.ToString()
                                              + "MasterEmailTemplate.cshtml";

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

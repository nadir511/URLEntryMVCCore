﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
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
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        private MimeMessage CreateEmailMessage(MessageVM message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email",_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
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
    }
}

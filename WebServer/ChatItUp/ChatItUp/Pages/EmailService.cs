using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.AspNetCore.Mvc;

namespace ChatItUp.Pages
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var gmailUsername = _configuration["EmailSettings:GmailUsername"];
            var gmailPassword = _configuration["EmailSettings:GmailPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Chat It Up Admin", fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "" + htmlMessage + "";
            message.Body = bodyBuilder.ToMessageBody();
            //message.Body = new TextPart("html") { Text = htmlMessage };

            using var smtpClient = new SmtpClient();
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtpClient.Connect("kbs-cloud.com", 25, false);
                smtpClient.Authenticate(gmailUsername, gmailPassword);
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage, MimeKit.AttachmentCollection attachments)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var gmailUsername = _configuration["EmailSettings:GmailUsername"];
            var gmailPassword = _configuration["EmailSettings:GmailPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ChatItUp Admin", fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "" + htmlMessage + "";
            foreach (var attachment in attachments)
                bodyBuilder.Attachments.Add(attachment);
            message.Body = bodyBuilder.ToMessageBody();
            
            //message.Body = new TextPart("html") { Text = htmlMessage };

            using var smtpClient = new SmtpClient();
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtpClient.Connect("kbs-cloud.com", 25, false);
                smtpClient.Authenticate(gmailUsername, gmailPassword);
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
            return Task.CompletedTask;
        }

        public async Task SendEmailActuallyAsync(string toEmail, string subject, string body)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var gmailUsername = _configuration["EmailSettings:GmailUsername"];
            var gmailPassword = _configuration["EmailSettings:GmailPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Chat It Up Admin", fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();
            //message.Body = new TextPart("html") { Text = body };

            using var smtpClient = new SmtpClient();
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtpClient.ConnectAsync("kbs-cloud.com", 25, false);
                await smtpClient.AuthenticateAsync(gmailUsername, gmailPassword);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}

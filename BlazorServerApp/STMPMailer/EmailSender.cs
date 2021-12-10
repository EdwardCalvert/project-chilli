using BlazorServerApp.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.STMPMailer
{
    public interface IEmailSender
    {
        public Task SendEmail(EmailMessage emailMessage);
    }

    public class EmailSender : IEmailSender

    {
        private readonly EmailSettings _mailSettings;
        private readonly IRecipeDataLoader _dataLoader;

        public EmailSender(EmailSettings mailSettings, IRecipeDataLoader dataLoader)
        {
            _mailSettings = mailSettings;
            _dataLoader = dataLoader;
        }

        public async Task SendEmail(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.From, _mailSettings.From));
            List<RecoveryEmailAddresses> addresses = await _dataLoader.GetEmailAddresses();
            message.To.AddRange(addresses.Select(x => new MailboxAddress(x.UserName, x.EmailAddress)));
            message.Subject = emailMessage.Subject;

            Console.WriteLine(message.To);

            message.Body = new TextPart("plain")
            {
                Text = emailMessage.Content
            };
            Console.WriteLine(message);
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_mailSettings.Username, _mailSettings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            Console.WriteLine("Attempted to send mail");
        }
    }
}
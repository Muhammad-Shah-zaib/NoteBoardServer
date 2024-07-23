using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NoteBoardServer.configuration;
using NoteBoardServer.Models.DTOs.Email;

namespace NoteBoardServer.services;

public class EmailService (IOptions<SmtpSettings> smtpSettings, IWebHostEnvironment env)
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
    private readonly IWebHostEnvironment _env = env;

    public async Task SendEmailAsync(MailDto mailDto)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            message.To.Add(new MailboxAddress(mailDto.Username, mailDto.ReceiverEmail));
            message.Subject = mailDto.Subject;
            message.Body = new TextPart("html")
            {
                Text = mailDto.Body
            };

            using (var client = new SmtpClient())
            {
                
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync( _smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }
}
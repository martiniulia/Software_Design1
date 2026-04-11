using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace FlowerShop.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient();
        
        client.Host = _config["Smtp:Host"];
        client.Port = int.Parse(_config["Smtp:Port"]);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(
            _config["Smtp:Username"],
            _config["Smtp:Password"]
        );

        var message = new MailMessage(
            from: _config["Smtp:FromAddress"],
            to: to,
            subject: subject,
            body: body
        );

        await client.SendMailAsync(message);
    }
}
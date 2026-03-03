#nullable enable
using FoodLovera.Core.Contracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace FoodLovera.Infrastructure.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
        => _config = config;

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct)
    {
        var s = _config.GetSection("Email:Smtp");

        var host = s["Host"] ?? throw new InvalidOperationException("Email:Smtp:Host missing");
        var port = int.TryParse(s["Port"], out var p) ? p : 587;
        var user = s["Username"] ?? throw new InvalidOperationException("Email:Smtp:Username missing");
        var pass = s["Password"] ?? throw new InvalidOperationException("Email:Smtp:Password missing");
        var from = (s["From"] ?? user).Trim();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("FoodLovera", from));
        message.To.Add(MailboxAddress.Parse(toEmail.Trim()));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();


        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(user.Trim(), pass.Trim(), ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
#nullable enable
namespace FoodLovera.Core.Contracts;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct);
}
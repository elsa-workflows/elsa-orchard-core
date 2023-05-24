using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using OrchardCore.Email;
using ISmtpService = Elsa.Email.Contracts.ISmtpService;
using IWrappedSmtpService = OrchardCore.Email.ISmtpService;

namespace Elsa.OrchardCore.Features.Emails.Services;

public class WrappingSmtpService : ISmtpService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public WrappingSmtpService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        // Map message to OrchardCore.Email.MailMessage
        var mailMessage = new MailMessage
        {
            From = message.From.ToString(FormatOptions.Default, false),
            To = message.To.ToString(FormatOptions.Default, false),
            Cc = message.Cc.ToString(FormatOptions.Default, false),
            Bcc = message.Bcc.ToString(FormatOptions.Default, false),
            Subject = message.Subject,
            Body = message.HtmlBody,
            IsBodyHtml = true,
            Sender = message.Sender.ToString(),
            ReplyTo = message.ReplyTo.ToString(),
        };

        foreach (var attachment in message.Attachments)
        {
            var attachmentStream = ((MimePart)attachment).Content.Stream;
            var filename = ((MimePart)attachment).FileName;
            mailMessage.Attachments.Add(new MailMessageAttachment
            {
                Stream = attachmentStream,
                Filename = filename
            });
        }

        using var scope = _scopeFactory.CreateScope();
        var wrappedSmtpService = scope.ServiceProvider.GetRequiredService<IWrappedSmtpService>();
        await wrappedSmtpService.SendAsync(mailMessage);
    }
}
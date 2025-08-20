using Gerenciamento.Shared.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EnvioWorker;

public class EmailSenderService : IEmailSenderService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailSenderService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(EmailSchedule email)
    {
        var retryDelays = new[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(25) };

        for (int i = 0; i < retryDelays.Length; i++)
        {
            try
            {
                if (i > 0)
                {
                    await Task.Delay(retryDelays[i]);
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", email.To));
                message.Subject = email.Subject;
                message.Body = new TextPart("html") { Text = email.Body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("E-mail ID: {EmailId} enviado com sucesso!", email.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha na tentativa {Attempt} de enviar o e-mail ID {EmailId}", i + 1, email.Id);
            }
        }
        _logger.LogError("Todas as {TotalAttempts} tentativas de envio para o e-mail ID {EmailId} falharam.", retryDelays.Length, email.Id);
        return false;
    }
}
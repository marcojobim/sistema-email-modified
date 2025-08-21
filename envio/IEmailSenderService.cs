using Gerenciamento.Shared.Models;

namespace EnvioWorker;

public interface IEmailSenderService
{
    Task<EmailSchedule?> SendEmailAsync(EmailSchedule email);
}
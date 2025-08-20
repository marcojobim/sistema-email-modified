using Gerenciamento.Shared.Models;

namespace EnvioWorker;

public interface IEmailSenderService
{
    Task<bool> SendEmailAsync(EmailSchedule email);
}
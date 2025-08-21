using Gerenciamento.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace EnvioWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de Envio iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

                    var emailsToSend = await dbContext.EmailSchedules.Where(e => e.SendTime <= DateTime.UtcNow && e.IsSent == false).ToListAsync(stoppingToken);

                    if (emailsToSend.Any())
                    {
                        var sendTasks = emailsToSend.Select(email => emailSender.SendEmailAsync(email));

                        var results = await Task.WhenAll(sendTasks);

                        var emailsSended = results.Where(email => email is not null).ToList();

                        if (emailsSended.Any())
                        {
                            foreach (var email in emailsSended)
                            {
                                email!.IsSent = true;
                            }

                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado no ciclo do worker");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
        _logger.LogInformation("Serviço de Envio encerrado");
    }
}

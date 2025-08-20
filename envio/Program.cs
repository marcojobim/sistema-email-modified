using EnvioWorker;
using Gerenciamento.Shared.Data;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
{
    services.Configure<SmtpSettings>(hostContext.Configuration.GetSection("SmtpSettings"));

    services.AddScoped<IEmailSenderService, EmailSenderService>();

    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connectionString));
    services.AddHostedService<Worker>();
}).Build();

host.Run();

using Gerenciamento.Shared.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("GerenciamentoApi")));

builder.Services.AddControllers();

builder.Services.AddHealthChecks().AddNpgSql(connectionString!);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiDbContext>();

    try
    {
        context.Database.Migrate();
        Console.WriteLine("---> Migrations aplicadas com sucesso");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as Migrations");
    }
}

app.MapHealthChecks("/healthz");

app.MapControllers();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();

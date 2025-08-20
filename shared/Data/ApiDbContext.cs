using Gerenciamento.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Gerenciamento.Shared.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }
    public DbSet<EmailSchedule> EmailSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var emailScheduleEntity = modelBuilder.Entity<EmailSchedule>();

        emailScheduleEntity.ToTable("email_schedules"); // Para não dar o problema das aspas

        emailScheduleEntity.HasIndex(e => e.SendTime).HasDatabaseName("IDX_email_schedules_send_date");

        emailScheduleEntity.Property(e => e.SendTime).HasColumnType("timestamp with time zone");

    }
}
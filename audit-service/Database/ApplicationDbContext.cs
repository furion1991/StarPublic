using AuditService.Database.Models;
using DtoClassLibrary.DataTransferObjects.Audit;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<BaseLog> BaseLogs { get; set; }
    public DbSet<UserLog> UserLogs { get; set; }
    public DbSet<CaseLog> CaseLogs { get; set; }
    public DbSet<ItemLog> ItemLogs { get; set; }
    public DbSet<FinancialLog> FinancialLogs { get; set; }
    public DbSet<OpenedCase> OpenedCases { get; set; }
    public DbSet<DailyServerStatistics> DailyServerStatistics { get; set; }
    public DbSet<BalanceStatisticsRecord> BalanceStatisticsRecords { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
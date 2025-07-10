using DtoClassLibrary.DataTransferObjects.Bonus;
using FinancialService.Database.Models;
using FinancialService.Database.Models.Bonuses;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<FinancialData> FinancialDatas { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Bonus> Bonuses { get; set; }
    public DbSet<UserBonusRecord> UserBonusRecords { get; set; }
    public DbSet<PaymentOrder> PaymentOrders { get; set; }
    public DbSet<PaymentProvider> PaymentProviders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserBonusRecord>().ToTable("users_bonuses");

        modelBuilder.Entity<UserBonusRecord>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<UserBonusRecord>()
            .HasOne(e => e.Bonus)
            .WithMany()
            .HasForeignKey(e => e.BonusId);

        modelBuilder.Entity<UserBonusRecord>()
            .HasOne(e => e.FinancialData)
            .WithMany(e => e.Bonuses)
            .HasForeignKey(e => e.FinDataId);

        modelBuilder.Entity<Bonus>()
            .ToTable("bonus")
            .HasDiscriminator<BonusType>("bonus_type")
            .HasValue<Bonus>(BonusType.None)
            .HasValue<BalanceBonus>(BonusType.BalanceBonus)
            .HasValue<CashbackBonus>(BonusType.CashbackBonus)
            .HasValue<DepositBonus>(BonusType.DepositBonus)
            .HasValue<DiscountBonus>(BonusType.DiscountBonus)
            .HasValue<FreeCaseBonus>(BonusType.FreeCaseBonus)
            .HasValue<ItemBonus>(BonusType.ItemBonus)
            .HasValue<RandomCaseBonus>(BonusType.RandomCaseBonus)
            .HasValue<WheelSpinBonus>(BonusType.FreeSpinBonus)
            .HasValue<LetterBonus>(BonusType.LetterBonus)
            .HasValue<FiveKBonus>(BonusType.FiveKBonus);

        modelBuilder.Entity<PaymentOrder>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<PaymentOrder>()
            .HasOne(e => e.Transaction)
            .WithOne(e => e.PaymentOrder)
            .HasForeignKey<PaymentOrder>(e => e.TransactionId);

        modelBuilder.Entity<PaymentProvider>()
            .HasKey(e => e.Id);

    }
}
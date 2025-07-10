using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UsersService.Models.DbModels;
using UsersService.Models.DbModels.MinorBonuses;

namespace UsersService.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<UserInventory> UserInventory { get; set; }
        public DbSet<UserStatistics> UserStatistics { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<BlockStatus> BlockStatus { get; set; }
        public DbSet<InventoryItemRecord> ItemsUser { get; set; }
        public DbSet<ContractHistoryRecord> ContractHistoryRecords { get; set; }
        public DbSet<DailyBonus> DailyBonuses { get; set; }
        public DbSet<PrizeDraw> PriceDraws { get; set; }
        public DbSet<PrizeDrawResult> PriceDrawResults { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(e => e.UserStatistics)
                .WithOne(e => e.User)
                .HasForeignKey<UserStatistics>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(e => e.UserInventory)
                .WithOne(e => e.User)
                .HasForeignKey<UserInventory>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(e => e.BlockStatus)
                .WithOne(e => e.User)
                .HasForeignKey<BlockStatus>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(e => e.UserRole)
                .WithOne(e => e.User)
                .HasForeignKey<UserRole>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserInventory>()
                .HasMany(u => u.InventoryRecords)
                .WithOne(i => i.UserInventory)
                .HasForeignKey(i => i.UserInventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserInventory>()
                .HasOne(u => u.User)
                .WithOne(u => u.UserInventory)
                .HasForeignKey<UserInventory>(u => u.UserId);


            modelBuilder.Entity<ContractHistoryRecord>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<ContractHistoryRecord>()
                .HasOne(e => e.User)
                .WithMany(e => e.ContractHistoryRecords)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractHistoryRecord>()
                .Property(e => e.ItemsFromIds)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>());


            modelBuilder.Entity<UpgradeHistoryRecord>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<UpgradeHistoryRecord>()
                .HasOne(e => e.User)
                .WithMany(e => e.UpgradeHistoryRecords)
                .HasForeignKey(e => e.UserId);


            modelBuilder.Entity<DailyBonus>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<DailyBonus>()
                .HasOne(e => e.User)
                .WithOne(e => e.DailyBonus)
                .HasForeignKey<DailyBonus>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PrizeDraw>()
                .HasMany(e => e.Participants)
                .WithOne(e => e.CurrentPriceDraw)
                .HasForeignKey(e => e.PriceDrawId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PrizeDrawResult>()
                .HasOne(e => e.WinnerUser)
                .WithMany(e => e.WonDraws)
                .HasForeignKey(e => e.Winner)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
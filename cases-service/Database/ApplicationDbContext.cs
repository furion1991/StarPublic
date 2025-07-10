using CasesService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CasesService.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<ItemCase> ItemsCases { get; set; }
    public DbSet<Multipliers> Multipliers { get; set; }
    public DbSet<CaseCategory> CaseCategories { get; set; }
    private ApplicationDbContext()
    {

    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Multipliers>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Multipliers>()
            .Property(e => e.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<ItemCase>()
            .HasOne(ic => ic.Item)
            .WithMany(i => i.ItemsCases)
            .HasForeignKey(ic => ic.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Case>()
            .HasOne(c => c.CaseCategory)
            .WithMany(e => e.Cases)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
using AuthService.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Database.Context;

public class AuthContext(DbContextOptions<AuthContext> options) : IdentityDbContext<StarDropUser>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ActiveUser> ActiveUsers { get; set; }
    public DbSet<TelegramUserData> TelegramUsers { get; set; }

    public DbSet<VkUserData> VkUserData { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<StarDropUser>(entity => { entity.ToTable("users"); });

        builder.Entity<IdentityRole>(entity => { entity.ToTable("roles"); });
        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("userroles"); });
        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("userclaims"); });
        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("userlogins"); });
        builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("roleclaims"); });
        builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("usertokens"); });

        builder.Entity<RefreshToken>()
            .HasKey(c => c.Id);

        builder.Entity<RefreshToken>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .IsRequired();

        builder.Entity<TelegramUserData>().ToTable("telegram_users");

        builder.Entity<TelegramUserData>()
            .HasKey(e => e.Id);

        builder.Entity<StarDropUser>()
            .HasOne(e => e.TelegramUserData)
            .WithOne(e => e.StarDropUser)
            .HasForeignKey<TelegramUserData>(e => e.MainUserId);

        builder.Entity<VkUserData>().ToTable("vk_user_data");

        builder.Entity<StarDropUser>()
            .HasOne(e => e.VkUserData)
            .WithOne(e => e.MainUser)
            .HasForeignKey<VkUserData>(e => e.MainUserId);
    }
}
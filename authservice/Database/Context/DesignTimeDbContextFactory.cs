using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService.Database.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthContext>
{
    public AuthContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=TestDb;Username=testuser;Password=testpassword");
        return new AuthContext(optionsBuilder.Options);
    }
}


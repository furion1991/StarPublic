using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthService.Database.Context;
using AuthService.Database.Models;
using AuthService.Services;
using DataTransferLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});
builder.Services.AddCommunicationServices();
builder.Services.AddScoped<JwtTokenService>();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
var connectionStringToBusiness = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_AUTH");
builder.Services.AddScoped<AuthService.Services.AuthService>();
builder.Services.AddHostedService<RabbitListenerForMail>();
builder.Services.AddScoped<EmailSenderService>();
builder.Services.AddDbContext<AuthContext>(options => { options.UseNpgsql(connectionStringToBusiness); });
builder.Services.AddDbContextFactory<AuthContext>(options => options.UseNpgsql(connectionStringToBusiness));
builder.Services.AddIdentity<StarDropUser, IdentityRole>(options => { options.SignIn.RequireConfirmedAccount = true; })
    .AddEntityFrameworkStores<AuthContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<TelegramSubscriptionChecker>();
builder.Services.AddScoped<VkService>();


var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMetricServer();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<AuthContext>();
if (context.Database.GetPendingMigrations().Any())
{
    context.Database.Migrate();
}

await CreateRolesAsync(services);

app.Run();

async Task CreateRolesAsync(IServiceProvider services)
{
    string[] roles = ["User", "Manager", "Admin"];

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in roles)
    {
        var roleExitst = await roleManager.RoleExistsAsync(roleName);
        if (!roleExitst)
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
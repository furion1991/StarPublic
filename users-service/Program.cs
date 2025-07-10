using DataTransferLib;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using UsersService;
using UsersService.Models;
using UsersService.HttpClientContext;
using UsersService.Models.DbModels;
using UsersService.Repositories;
using UsersService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("https://24cases.ru", "http://localhost:3000", "https://stardrop.vercel.app")
            .AllowAnyMethod()
            .AllowCredentials()
            .WithHeaders(
                "Authorization",
                "Content-Type",
                "X-Requested-With",
                "X-SignalR-User-Agent"
            );
    });
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<HttpClientService>();
var connectionStringToBusiness = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_USERS");
builder.Services.AddScoped<UserService>();
builder.Services.AddDbContextFactory<ApplicationDbContext>(option => { option.UseNpgsql(connectionStringToBusiness); });
builder.Services.AddScoped<SmallBonusService>();
builder.Services.AddScoped<IUserServiceRepository<User>, UserUserServiceRepository>();
builder.Services.AddScoped<IPrizeUserServiceRepository, PrizeUserServiceDrawRepository>();
builder.Services.AddScoped<IPrizeDrawResultUserServiceRepository, PrizeDrawResultsUserServiceRepository>();
builder.Services.AddScoped<PrizeDrawService>();
builder.Services.AddCommunicationServices();
builder.Services.AddHostedService<PrizeDrawBroadcastService>();
builder.Services.AddSignalR();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseRouting();

app.UseAuthorization();
app.UseMetricServer();
app.MapControllers();

app.MapRazorPages();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<ApplicationDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    context.Database.Migrate();
}

app.MapHub<PrizeDrawSender>("/draw", options => { options.Transports = HttpTransportType.ServerSentEvents; });
app.Run();
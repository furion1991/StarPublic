using AuditService.Database;
using AuditService.Database.Models;
using AuditService.Repositories;
using AuditService.Services;
using DataTransferLib;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;

using Prometheus;

using Serilog;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCommunicationServices();
builder.Services.AddSignalR();

var connectionStringToBusiness = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_AUDIT");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(
        connectionStringToBusiness));

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<IRepository<BaseLog>, BaseLogRepository>();
builder.Services.AddScoped<IRepository<CaseLog>, CaseLogRepository>();
builder.Services.AddScoped<IRepository<ItemLog>, ItemLogRepository>();
builder.Services.AddScoped<IRepository<FinancialLog>, FinancialLogRepository>();
builder.Services.AddScoped<IRepository<UserLog>, UserLogRepository>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<OpenedCasesService>();
builder.Services.AddScoped<FinLogService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionStringToBusiness));
builder.Services.AddHostedService<ListenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("CorsPolicy");
app.UseRouting();
app.UseMetricServer();
app.UseAuthorization();


app.MapControllers();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<ApplicationDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    context.Database.Migrate();
}

app.MapHub<Sender>("/statistics", options => { options.Transports = HttpTransportType.ServerSentEvents; });
app.MapHub<DashboardHub>("/dashboard", options => { options.Transports = HttpTransportType.ServerSentEvents; });


await Task.Delay(5000);
app.Run();

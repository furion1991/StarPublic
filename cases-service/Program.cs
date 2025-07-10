using Microsoft.EntityFrameworkCore;
using CasesService.Database;
using Prometheus;
using Serilog;
using CasesService.Database.Models;
using CasesService.Repositories;
using CasesService.Services;
using DataTransferLib;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using RabbitMQ.Client;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.CasesItems;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommunicationServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var connectionStringToBusiness = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_CASES");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(
        connectionStringToBusiness));

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<IRepository<Case>, CaseRepository>();
builder.Services.AddScoped<IRepository<Item>, ItemRepository>();
builder.Services.AddScoped<IRepository<ItemCase>, ItemCaseRepository>();
builder.Services.AddScoped<CaseOpenService>();
builder.Services.AddScoped<ContractService>();
builder.Services.AddScoped<UpgradeService>();




var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
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
app.Run();
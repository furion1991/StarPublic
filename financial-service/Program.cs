using DataTransferLib;
using Microsoft.EntityFrameworkCore;
using FinancialService.Database;
using Prometheus;
using Serilog;
using FinancialService.Repositories;
using FinancialService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.Converters;
using FinancialService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new TransactionJsonConverter());
});
builder.Services.AddCommunicationServices();


var connectionStringToBusiness = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_FIN");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
    connectionStringToBusiness));

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<IRepository<FinancialData>, FinancialDataRepository>();
builder.Services.AddScoped<IRepository<Transaction>, TransactionRepository>();
builder.Services.AddScoped<BonusService>();
builder.Services.AddScoped<IPaymentService, WataPaymentService>();
builder.Services.AddScoped<WataPaymentService>();
builder.Services.AddScoped<PaymentService>();


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
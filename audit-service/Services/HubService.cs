using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.CommunicationsServices;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace AuditService.Services
{
    public class Sender(
        IDbContextFactory<ApplicationDbContext> factory,
        AuthCommService authCommService,
        CasesCommService casesCommService,
        OpenedCasesService openedCasesService, ILogger<Sender> logger) : Hub
    {
        public const string LastOpenedCasesByCost = "cases_logs_by_cost";
        public const string LastOpenedCases = "cases_logs";
        public const string LoggedInUsers = "users_logged_in";
        public const string CasesOpenedCount = "cases_opened_count";

        public async Task Send(string name, object message) =>
            await Clients.All.SendAsync(name, message);

        public override async Task OnConnectedAsync()
        {
            logger.LogInformation($"Connected client {Clients.Caller.ToJson()}");
            var dbContext = await factory.CreateDbContextAsync();

            var lastItemDroppedList = await openedCasesService.GetLastTenDroppedItems();
            var lastItemDroppedListByCost = await openedCasesService.GetLastTenItemsDroppedByCost();

            var casesOpenedCount = await dbContext.OpenedCases.CountAsync();
            var activeUsers = await authCommService.GetActiveUsers();
            await Clients.Caller.SendAsync(CasesOpenedCount, casesOpenedCount);
            await Clients.Caller.SendAsync(LastOpenedCases, lastItemDroppedList);
            await Clients.Caller.SendAsync(LastOpenedCasesByCost, lastItemDroppedListByCost);
            await Clients.Caller.SendAsync(LoggedInUsers, activeUsers);

            await base.OnConnectedAsync();
        }
    }
}